using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcAI : MonoBehaviour
{
    public static event System.Action<Transform> OnNewNpcAdded;

    public NpcMovement npcMovement;
    public Inventory inventory;
    private Animator animator;
    private WeaponDetails weaponDetails;
    public NpcHealth npcHealth;
    public NpcAttack npcAttack;
    public NpcTargeting npcTargeting;
    public NpcUtil npcUtil;

    public LootSpawner lootSpawner; // Add a reference to the LootSpawner component

    public enum State
    {
        Roaming,
        Following,
        Attacking,
        Dead,
        ManualControl 
    }

    public State state;
    public State CurrentState => state;

    private void Awake()
    {
         inventory = new Inventory();
        animator = GetComponent<Animator>();
        weaponDetails = GetComponentInChildren<WeaponDetails>();
        npcHealth = GetComponent<NpcHealth>();
        npcAttack = GetComponent<NpcAttack>();
        npcMovement = GetComponent<NpcMovement>();
        npcTargeting = GetComponent<NpcTargeting>();
        npcUtil = GetComponent<NpcUtil>();

        lootSpawner = GetComponent<LootSpawner>(); // Get the LootSpawner component

        // state = State.Roaming;
    }

    private void Start()
    { 
        // npcMovement.roamPosition = npcMovement.GetRoamingPosition();

        npcTargeting.FindPotentialTargets(this);

        if (OnNewNpcAdded != null)
        {
            OnNewNpcAdded(transform);
        }

        npcTargeting.target = npcTargeting.GetClosestTarget(transform, npcMovement.detectRange);
        // Debug.Log("weaponDetails" + weaponDetails.damageAmount);
    }

    private void Update()
    {
        MovementStateControl();
        // npcMovement.CheckIfStuck();
        npcTargeting.FindNewTargetIfNeeded();
    }

    private void OnDestroy()
    {
        npcTargeting.OnDestroy();
    }

    private void MovementStateControl()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                npcMovement.Roaming();
                break;
            case State.Following:
                npcMovement.Following();
                break;
            case State.ManualControl:
                // npcMovement.Following();
                break;
            case State.Attacking:
                npcAttack.Attacking();
                break;
            case State.Dead:
                Dead(null);
                break;
        }
    }

  public void Dead(GameObject killer)
{
    if (state == State.Dead) return;

    state = State.Dead;
    npcMovement.isMoving = false;
    npcAttack.isAttacking = false;
    npcAttack.canAttack = true;

    Collider2D collider = GetComponent<Collider2D>();
    if (collider != null)
    {
        collider.enabled = false;
    }

    animator.SetTrigger("Die");

    // Debug.Log($"{killer.name} dealt the killing blow to {gameObject.name}");

    // Drop loot items
    if (lootSpawner != null)
    {
        // Debug.Log("LootSpawner is not null. Attempting to drop loot.");
        lootSpawner.DropItems(killer.transform);
    }
    else
    {
        Debug.LogError("LootSpawner is null! No loot will be spawned.");
    }

    Destroy(gameObject, 10f);
}




}
