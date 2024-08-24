using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcAttack : MonoBehaviour
{
    private Animator animator;  
    private PolygonCollider2D weaponCollider;
    private HashSet<Collider2D> hitObjects;
    private NpcAI npcAI;
    private WeaponDetails weaponDetails;
    public Transform target;
    public bool canAttack = true;
    public bool isAttacking = false;

    private float targetDistance;

    public GameObject arrowPrefab; // Prefab for the arrow
    public Transform arrowSpawnPoint; // Spawn point for the arrow


    public delegate void WeaponDetailsUpdated(WeaponDetails newWeaponDetails);
    public event WeaponDetailsUpdated OnWeaponDetailsUpdated;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        weaponCollider = GetComponentInChildren<PolygonCollider2D>();
        npcAI = GetComponent<NpcAI>();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }


    

    private void Start()
    {
        hitObjects = new HashSet<Collider2D>();
        UpdateWeaponDetails(); // Initial update of weapon details
    }

   private void UpdateWeaponDetails()
    {
        Transform activeWeapon = transform.Find("ActiveWeapon");
        if (activeWeapon != null)
        {
            foreach (Transform child in activeWeapon)
            {
                if (child.gameObject.activeSelf)
                {
                    weaponDetails = child.GetComponent<WeaponDetails>();
                    if (weaponDetails == null)
                    {
                        Debug.LogError("WeaponDetails component is missing on the active weapon.");
                    }
                    else
                    {
                        OnWeaponDetailsUpdated?.Invoke(weaponDetails);
                    }
                    break;
                }
            }
        }
    }



 
  public void Attacking()
{
    if (weaponDetails == null)
    {
        return;
    }

    target = npcAI.npcTargeting.target;

    if (target != null)
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        NpcAI targetAI = target.GetComponent<NpcAI>();

        // Handle target dead state
        if (targetAI != null && targetAI.state == NpcAI.State.Dead)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            target = npcAI.npcTargeting.GetClosestTarget(transform, npcAI.npcMovement.detectRange);
            npcAI.state = target != null ? NpcAI.State.Following : NpcAI.State.Roaming;
            npcAI.npcMovement.roamPosition = npcAI.npcMovement.GetRoamingPosition();
            npcAI.npcTargeting.RemoveTarget(target);
            return;
        }

        // If target is outside detect range, stop attack and roam
        if (distanceToTarget > npcAI.npcMovement.detectRange)
        {
            npcAI.state = NpcAI.State.Roaming;
            EndAttack();
            npcAI.npcMovement.roamPosition = npcAI.npcMovement.GetRoamingPosition();
            return;
        }

        // Update direction before attacking
        // UpdateDirection();

        // If the target is within range, start attacking
        if (distanceToTarget <= weaponDetails.range && canAttack)
        {
            npcAI.npcMovement.npcPathfinding.StopMoving();
            canAttack = false;
            StartCoroutine(PerformAttack());
        }
        // If the target is outside weapon range but inside detect range, follow target after the attack ends
        else if (!isAttacking)
        {
            npcAI.state = NpcAI.State.Following;
            npcAI.npcMovement.StartFollowingTarget();
        }
    }
    else
    {
        // If no target, switch to roaming
        npcAI.state = NpcAI.State.Roaming;
        npcAI.npcMovement.roamPosition = npcAI.npcMovement.GetRoamingPosition();
    }
}


    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        hitObjects.Clear();
    }

    private void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    private void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private IEnumerator PerformAttack()
{
    if (weaponDetails == null)
    {
        yield break;
    }

    npcAI.npcMovement.isMoving = false; // Ensure movement is disabled during attack
    animator.SetBool("isMoving", false);
    hitObjects.Clear();

    // Update the direction before starting the attack
    // UpdateDirection();

    SetAttackType();
    isAttacking = true;

    animator.SetBool("isAttacking", true);
    animator.SetFloat("AttackSpeed", weaponDetails.attackSpeed);

    yield return new WaitForSeconds(1 / weaponDetails.attackSpeed);

    EndAttack();

    yield return new WaitForSeconds(weaponDetails.cooldown);

    canAttack = true;

    if (npcAI.state == NpcAI.State.Attacking)
    {
        npcAI.state = target != null ? NpcAI.State.Following : NpcAI.State.Roaming;
        if (target != null)
        { 
            UpdateAnimator(((Vector2)target.position - (Vector2)transform.position));
        }
        else
        {
            Debug.Log("target null for " + gameObject.name);
        }
    } else{
        UpdateAnimator(((Vector2)target.position - (Vector2)transform.position));
    }
}
 

    private void UpdateAnimator(Vector2 direction)
    {     
        animator.SetFloat("MoveX", Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0);
        animator.SetFloat("MoveY", Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0);
        animator.SetBool("isMoving", direction.magnitude > 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (weaponCollider != null && weaponCollider.enabled && !hitObjects.Contains(other))
        {
            if (other is BoxCollider2D)
            {
                NpcHealth targetHealth = other.gameObject.GetComponent<NpcHealth>();
                if (targetHealth != null && other.transform == target)
                 {
                hitObjects.Add(other);
                targetHealth.TakeDamage(weaponDetails.damageAmount, gameObject); // Pass this GameObject as the attacker
            }
            }
        }
    }

    private void InstantiateArrow()
    {
        if (weaponDetails == null)
        {
            return;
        }

        if (arrowPrefab != null && arrowSpawnPoint != null && target != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Vector3 direction = (target.position - arrowSpawnPoint.position).normalized;
            NpcProjectile npcProjectile = newArrow.GetComponent<NpcProjectile>();
            npcProjectile.UpdateProjectileRange(npcAI.npcMovement.detectRange);
            npcProjectile.SetDirection(direction);
            npcProjectile.SetDamage(weaponDetails.damageAmount);

            Collider2D instantiatorCollider = GetComponent<Collider2D>();
            if (instantiatorCollider != null)
            {
                npcProjectile.SetInstantiatorCollider(instantiatorCollider.name);
            }
            else
            {
                Debug.LogError("Instantiator collider is not found.");
            }
        }
        else
        {
            Debug.Log("Either arrowPrefab, arrowSpawnPoint, or target is null");
        }
    }

    private void SetAttackType()
    {
        animator.ResetTrigger("SwordAttack");
        animator.ResetTrigger("BowAttack");
        animator.ResetTrigger("SpearAttack");

        Transform activeWeapon = transform.Find("ActiveWeapon");
        if (activeWeapon != null)
        {
            foreach (Transform child in activeWeapon)
            {
                if (child.gameObject.activeSelf)
                {
                    switch (child.tag)
                    {
                        case "Sword":
                            animator.SetTrigger("SwordAttack");
                            break;
                        case "Bow":
                            animator.SetTrigger("BowAttack");
                            break;
                        case "Spear":
                            animator.SetTrigger("SpearAttack");
                            break;
                        default:
                            Debug.Log("Unknown weapon type");
                            break;
                    }
                    UpdateWeaponDetails(); // Update weapon details when the weapon changes
                    break;
                }
            }
        }
    }
}
