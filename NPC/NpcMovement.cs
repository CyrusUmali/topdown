using UnityEngine;
using System.Collections;

public class NpcMovement : MonoBehaviour
{
    public float roamChangeDirTime = 2f;
    public float detectRange = 10f;
    public float followInterval = 2f;
    private bool isFollowing = false;
    public float updatePathThreshold = 1f;
    public float stuckCheckInterval = 2f;
    public float stuckDistanceThreshold = 0.1f;

    public Vector2 roamPosition;
    private float timeRoaming = 0f;

    
    public float roamInterval = 9f;
    private bool isWaitingForRoam = false;
    public bool isMoving = false;

    private Animator animator;
    public NpcPathfinding npcPathfinding;
    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private Vector3 lastPosition;
    private float stuckTimer;
    private NpcAI npcAI;

    private NpcAttack npcAttack;
    private WeaponDetails weaponDetails;
    private bool isAttacking = false; // Assuming this is determined elsewhere
    private bool isControlled;


     public float nextWaypointDistance = 0.1f;

    private void Awake()
    {
        npcPathfinding = GetComponent<NpcPathfinding>();
        animator = GetComponent<Animator>();
        npcAI = GetComponent<NpcAI>();
        weaponDetails = GetComponentInChildren<WeaponDetails>();
         npcAttack = GetComponent<NpcAttack>();
    }

    private void Start()
    {
        // roamPosition = GetRoamingPosition();
        lastPosition = transform.position;
        stuckTimer = stuckCheckInterval;

       
        if (npcAttack != null)
        {
            npcAttack.OnWeaponDetailsUpdated += HandleWeaponDetailsUpdated;
        }
    }

    private void HandleWeaponDetailsUpdated(WeaponDetails newWeaponDetails)
    {
        weaponDetails = newWeaponDetails;
    }

    private void OnDestroy()
    {
        NpcAttack npcAttack = GetComponent<NpcAttack>();
        if (npcAttack != null)
        {
            npcAttack.OnWeaponDetailsUpdated -= HandleWeaponDetailsUpdated;
        }
    }

    private void Update()
    {
        if (SelectedNpcManager.Instance.GetCurrentNpcName() == gameObject.name   )
        {
            HandleManualMovement();
            
        }
        CheckIfStuck();
    }
    private void HandleManualMovement()
    {
    if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl) && !isAttacking && npcAI.state != NpcAI.State.Dead) 
    {
        npcAttack.EndAttack();
        isControlled = true;
        SetManualControl();
        
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = transform.position.z;
        isMoving = true;

        npcPathfinding.StartPath(targetPosition);
    }

    if (isMoving)
    {
            // Debug.Log("isMoving"+isMoving);
        if (npcPathfinding.isPathfindingComplete) // Check if pathfinding is complete
        {
                // Debug.Log(" if (npcPathfinding.isPathfindingComplete) "+npcPathfinding.isPathfindingComplete);
            if (npcPathfinding.hasPath)
            {
                // Debug.Log("(npcPathfinding.hasPath) " + npcPathfinding.hasPath);
                npcPathfinding.FollowPath();

                Vector3 currentWaypoint = npcPathfinding.GetCurrentWaypoint();
                    // Debug.Log("currentWaypoint"+currentWaypoint);
                Vector3 direction = (currentWaypoint - transform.position).normalized;

                UpdateAnimator(direction); 
      
                animator.SetBool("isMoving", true); 

            }
            else
            {
                // Debug.Log("(npcPathfinding.hasPath) else: " + npcPathfinding.hasPath);
                isMoving = false;
                isControlled = false;
                SetManualControl();
                npcPathfinding.StopMoving();
                Debug.Log("No valid path");
                animator.SetBool("isMoving", false);
            }
        }
       
       
         if (npcPathfinding.pathReached)
            {
                    isMoving = false;
                    animator.SetBool("isMoving", false);
                    Debug.Log("Target reached");
                    isControlled = false;
                    SetManualControl();
                      return;
                    // Optionally, handle any additional logic here
             }
    }
}

    
    public void SetManualControl()
{
    if (isControlled)
    {
        npcAI.state = NpcAI.State.ManualControl; 
        // StopAllCoroutines();  
    }
    else
    {
        StartCoroutine(RevokeManualControl(5f,
         npcAI.state = npcAttack.target != null ? NpcAI.State.Following : NpcAI.State.Roaming));
    }
}

    private IEnumerator RevokeManualControl(float delay, NpcAI.State newState)
{
    yield return new WaitForSeconds(delay);
    npcAI.state = newState;
}




    public void Roaming()
         {
        timeRoaming += Time.deltaTime; 
    //     npcPathfinding.StartPath(roamPosition);

         if (isMoving)
            {
            // Debug.Log("isMoving"+isMoving);
          if (npcPathfinding.isPathfindingComplete) // Check if pathfinding is complete
        {
                // Debug.Log(" if (npcPathfinding.isPathfindingComplete) "+npcPathfinding.isPathfindingComplete);
            if (npcPathfinding.hasPath)
            {
                // Debug.Log("(npcPathfinding.hasPath) " + npcPathfinding.hasPath);
                npcPathfinding.FollowPath();

                Vector3 currentWaypoint = npcPathfinding.GetCurrentWaypoint();
                    // Debug.Log("currentWaypoint"+currentWaypoint);
                Vector3 direction = (currentWaypoint - transform.position).normalized;

                UpdateAnimator(direction); 
      
                animator.SetBool("isMoving", true); 

            }
            else
            {
                // Debug.Log("(npcPathfinding.hasPath) else: " + npcPathfinding.hasPath);
                isMoving = false;
                // isControlled = false;
                // SetManualControl();
                npcPathfinding.StopMoving();
                Debug.Log("No valid path");
                animator.SetBool("isMoving", false);
            }
        }
       
       
         if (npcPathfinding.pathReached)
            {
                    isMoving = false;
                    animator.SetBool("isMoving", false);
                    Debug.Log("Target reached");
                    // isControlled = false;
                    // SetManualControl();
                      return;
                    // Optionally, handle any additional logic here
             }
     }

        // if (npcPathfinding != null)
        // {
        //     npcPathfinding.MoveTo(roamPosition);
        //     UpdateAnimator(roamPosition - (Vector2)transform.position);
        // } 



        if (npcAI != null && npcAI.npcTargeting != null && npcAI.npcTargeting.target != null)
        {
            float targetDistance = Vector2.Distance(transform.position, npcAI.npcTargeting.target.position);
            if (weaponDetails != null && targetDistance < weaponDetails.range)
            {
                npcAI.state = NpcAI.State.Attacking;
            }
            else if (targetDistance < detectRange)
            {
                if (!isFollowing)
                {
                    npcAI.state = NpcAI.State.Following;
                    StartFollowingTarget();
                    StartCoroutine(FollowCoroutine());
                }
            }
        }
    
        if (!isWaitingForRoam && timeRoaming > roamChangeDirTime)
         {
             // Debug.Log("Starting coroutine with roamInterval: " + roamInterval);
           StartCoroutine(SetNewRoamingPositionWithDelay());
        }
    }


    private IEnumerator SetNewRoamingPositionWithDelay()
{
    isWaitingForRoam = true; // Indicate coroutine is running
    // Debug.Log("Coroutine Started with interval: " + roamInterval);

    // npcPathfinding.StopMoving();

    // Wait for the defined interval before changing the roaming position
    yield return new WaitForSeconds(roamInterval);

    // Debug.Log("Coroutine Ended with interval: " + roamInterval);

    roamPosition = GetRoamingPosition(); 
    timeRoaming = 0f; // Reset the roaming time after changing position

    isWaitingForRoam = false; // Indicate coroutine is finished
}

    public void Following()
    {
        if (npcAI != null && npcAI.npcTargeting != null && npcAI.npcTargeting.target != null)
        {
            Vector3 targetPosition = npcAI.npcTargeting.target.position;
            float targetDistance = Vector2.Distance(transform.position, targetPosition);

            if (Vector2.Distance(lastTargetPosition, targetPosition) > updatePathThreshold)
            {
                StartFollowingTarget();
                lastTargetPosition = targetPosition;
            }

            if (targetDistance > detectRange)
            {
                npcAI.state = NpcAI.State.Roaming;
                roamPosition = GetRoamingPosition();
            }
            else if (weaponDetails != null && targetDistance < weaponDetails.range)
            {
                npcAI.state = NpcAI.State.Attacking;
            }
            else if (!npcPathfinding.hasPath)
            {
                npcAI.state = NpcAI.State.Roaming;
                roamPosition = GetRoamingPosition();
            }
            else
            {
                npcPathfinding.FollowPath();
                UpdateAnimator((Vector2)targetPosition - (Vector2)transform.position);
            }
        }
        else
        {
            npcAI.state = NpcAI.State.Roaming;
            roamPosition = GetRoamingPosition();
        }
    }

    public void StartFollowingTarget()
    {
        if (npcAI != null && npcAI.npcTargeting != null && npcAI.npcTargeting.target != null)
        {
            Vector3 targetPosition = npcAI.npcTargeting.target.position;
            npcPathfinding.StartPath(targetPosition);
            lastTargetPosition = targetPosition;
        }
    }

    public Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;

        if (npcAI != null && npcAI.state == NpcAI.State.Dead)
        {
            return transform.position;
        }

        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector2 targetPosition = (Vector2)transform.position + randomDirection * roamChangeDirTime * npcPathfinding.moveSpeed;
        isMoving = true;
        npcPathfinding.StartPath(targetPosition);


        return targetPosition;
    }
 

    public void UpdateAnimator(Vector2 direction)
    {
    // Only update direction if there is significant movement
    if (direction.magnitude > 0.1f)
    {
        animator.SetFloat("MoveX", Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0);
        animator.SetFloat("MoveY", Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0);
        animator.SetBool("isMoving", direction.magnitude > 0.1f);
    }
    }

    private IEnumerator FollowCoroutine()
    {
        isFollowing = true;
        yield return new WaitForSeconds(followInterval);
        isFollowing = false;
    }

  public void CheckIfStuck()
{
    stuckTimer -= Time.deltaTime;
    if (stuckTimer <= 0f && (npcAI.state != NpcAI.State.Attacking))
    {
        float distanceMoved = Vector2.Distance(lastPosition, transform.position);
        if (distanceMoved < stuckDistanceThreshold)
        {
            if (npcAI.state == NpcAI.State.Roaming)
            {
                Debug.Log("Stuck Roaming");
                 Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                // Adjust the collider's y offset
                Vector2 originalOffset = collider.offset;
                Vector2 newOffset = originalOffset + new Vector2(0, -0.2f); // Adjust the offset as needed

                collider.offset = newOffset;
                // Debug.Log($"Collider offset adjusted to: {newOffset}");

                // Start coroutine to reset the offset
                StartCoroutine(ResetColliderOffset(collider, originalOffset));
            }
            else
            {
                Debug.Log("Collider is null");
            }
            }

             if (npcAI.state == NpcAI.State.ManualControl)
              { 

                 isMoving = false;
                isControlled = false;
                SetManualControl();
                npcPathfinding.StopMoving();
                Debug.Log("No valid path");
                animator.SetBool("isMoving", false);
                Debug.Log("Stuck manual");
                }

           
        }
        lastPosition = transform.position;
        stuckTimer = stuckCheckInterval;
    }
}

private IEnumerator ResetColliderOffset(Collider2D collider, Vector2 originalOffset)
{
    yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
    collider.offset = originalOffset;
    // Debug.Log($"Collider offset reset to: {originalOffset}");
}


}
