using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackSpeed = 1f;
    public float detectionRange = 1f;
    public float attackCooldown = 0.5f;

    public GameObject arrowPrefab; // Prefab for the arrow
    public Transform arrowSpawnPoint; // Spawn point for the arrow

    private Animator animator;
    private PolygonCollider2D weaponCollider;
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isAttacking;
    private HashSet<Collider2D> hitObjects;
    private Transform targetDummy; // Reference to the target dummy

    private Seeker seeker;
    private Path path;
    private int currentWaypoint;
    public float nextWaypointDistance = 0.1f; // The distance to the next waypoint before moving to the next one

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponCollider = GetComponentInChildren<PolygonCollider2D>();
        targetPosition = transform.position;
        hitObjects = new HashSet<Collider2D>();

        seeker = GetComponent<Seeker>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        LogActiveWeaponTag();
        UpdateDetectionRange(); // Update detection range at start
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            isMoving = true;

            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }

        if (isMoving && path != null)
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {
                isMoving = false;
                animator.SetBool("isMoving", false);
                return;
            }

            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, path.vectorPath[currentWaypoint], moveSpeed * Time.deltaTime);

            SetAnimatorDirection(direction);
            animator.SetBool("isMoving", true);

            float distance = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }

        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.A)) && !isAttacking)
        {
            FaceNearbyDummy();
            StartCoroutine(PerformAttack());
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private IEnumerator PerformAttack()
    {
        StopMovementAndAttack();
        EnableWeaponCollider();
        yield return new WaitForSeconds(attackSpeed);
        DisableWeaponCollider();
        EndAttack();
    }

    private void StopMovementAndAttack()
    {
        isMoving = false;
        animator.SetBool("isMoving", false);
        targetPosition = transform.position;
        hitObjects.Clear();
        FaceNearbyDummy();
        animator.SetFloat("AttackSpeed", attackSpeed);
        SetAttackType();
        isAttacking = true;
        animator.SetBool("isAttacking", true);
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
                            // InstantiateArrow(); 
                            break;
                        case "Spear":
                            animator.SetTrigger("SpearAttack");
                            break;
                        default:
                            Debug.Log("Unknown weapon type");
                            break;
                    }
                    break;
                }
            }
        }
        UpdateDetectionRange(); // Update detection range when the active weapon is set
    }

    private void InstantiateArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null && targetDummy != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Vector3 direction = (targetDummy.position - arrowSpawnPoint.position).normalized;
            newArrow.GetComponent<NpcProjectile>().UpdateProjectileRange(detectionRange);
            newArrow.GetComponent<NpcProjectile>().SetDirection(direction); // Set the direction of the arrow
        }
        else
        {
            Debug.Log("Either arrowPrefab, arrowSpawnPoint, or targetDummy is null");
        }
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

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        hitObjects.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (weaponCollider.enabled && !hitObjects.Contains(other))
        {
            Dummy dummy = other.GetComponent<Dummy>();
            if (dummy != null)
            {
                hitObjects.Add(other);
                Debug.Log("Hit a dummy!");
            }
        }
    }

    private void FaceNearbyDummy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            Dummy dummy = hitCollider.GetComponent<Dummy>();
            if (dummy != null)
            {
                targetDummy = dummy.transform; // Store the target dummy
                Vector3 direction = (dummy.transform.position - transform.position).normalized;
                SetAnimatorDirection(direction);
                break;
            }
        }
    }

    private void SetAnimatorDirection(Vector3 direction)
    {
        animator.SetFloat("MoveX", Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0);
        animator.SetFloat("MoveY", Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void LogActiveWeaponTag()
    {
        Transform activeWeapon = transform.Find("ActiveWeapon");
        if (activeWeapon != null)
        {
            foreach (Transform child in activeWeapon)
            {
                if (child.gameObject.activeSelf)
                {
                    Debug.Log("Active Weapon Tag: " + child.tag);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("ActiveWeapon GameObject not found");
        }
    }

    private void UpdateDetectionRange()
    {
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
                            detectionRange = 1f;
                            break;
                        case "Bow":
                            detectionRange = 6f;
                            break;
                        case "Spear":
                            detectionRange = 1.5f;
                            break;
                        default:
                            Debug.Log("Unknown weapon type");
                            break;
                    }
                    break;
                }
            }
        }
    }
}
