    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private float roamChangeDirFloat = 2f;
        [SerializeField] private float attackRange = 5f;
        [SerializeField] private float detectRange = 10f; // New detection range
        [SerializeField] private MonoBehaviour enemyType;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private bool stopMovingWhileAttacking = false;

        public Transform target;

        private bool canAttack = true;

        private enum State {
            Roaming,
            Following, // New state
            Attacking
        }

        private Vector2 roamPosition;
        private float timeRoaming = 0f;

        private State state;
        private EnemyPathfinding enemyPathfinding;

        private void Awake() {
            enemyPathfinding = GetComponent<EnemyPathfinding>();
            if (enemyPathfinding == null) {
                Debug.LogError("EnemyPathfinding component is missing!");
            }
            state = State.Roaming;
        }

        private void Start() {
            // roamPosition = GetRoamingPosition(); 
            Attacking(); 
        }

        private void Update() {
            MovementStateControl(); 
        }

        private void MovementStateControl() {
            switch (state)
            {
                default:
                case State.Roaming:
                    Roaming();
                    break;

                case State.Following:
                    Following();
                    break;

                case State.Attacking:
                    Attacking();
                    break;
            }
        }

        private void Roaming() {
            timeRoaming += Time.deltaTime;

            if (enemyPathfinding != null) {
                enemyPathfinding.MoveTo(roamPosition);
            }

            if (target != null) {
                float playerDistance = Vector2.Distance(transform.position, target.transform.position);
                // Debug.Log("Roaming: Player distance: " + playerDistance);

                if (playerDistance < attackRange) {
                    Debug.Log("Switching to Attacking state");
                    state = State.Attacking;
                } else if (playerDistance < detectRange) {
                    Debug.Log("Switching to Following state");
                    state = State.Following;
                }
            }

            if (timeRoaming > roamChangeDirFloat) {
                roamPosition = GetRoamingPosition();
            }
        }

        private void Following() {
            if (target != null) {
                Vector3 playerPosition = target.transform.position;
                float playerDistance = Vector2.Distance(transform.position, playerPosition);
                // Debug.Log("Following: Player distance: " + playerDistance);
                // Debug.Log("Following: Player position: " + playerPosition);

                if (playerDistance < attackRange) {
                    Debug.Log("Switching to Attacking state");
                    state = State.Attacking;
                } else if (playerDistance > detectRange) {
                    Debug.Log("Switching to Roaming state");
                    state = State.Roaming;
                    roamPosition = GetRoamingPosition();
                } else {
        enemyPathfinding.followPlayer(playerPosition); 
                    // Debug.Log("Following player to position: " + playerPosition);
                }
            } else {
                Debug.Log("target  = null");
                state = State.Roaming;
                roamPosition = GetRoamingPosition();
            }
        }

        private void Attacking() {
  

             // If the player exists and is farther than the detect range
    if (target != null && Vector2.Distance(transform.position, target.transform.position) > detectRange) {
        Debug.Log("Switching to Roaming state");
        state = State.Roaming;
        roamPosition = GetRoamingPosition();
    }
    // If the player exists and is within the detect range but farther than the attack range
    else if (target != null && Vector2.Distance(transform.position, target.transform.position) > attackRange) {
        Debug.Log("Switching to Following state");
        state = State.Following;
    }

            if (attackRange > 0 && canAttack) {
                canAttack = false;

                if (enemyType != null) {
                    (enemyType as IEnemy)?.Attack();
                    Debug.Log("here.");
                } else {
                    Debug.LogWarning("EnemyType is not assigned or does not implement IEnemy.");
                }

                if (stopMovingWhileAttacking && enemyPathfinding != null) {
                    enemyPathfinding.StopMoving();
                    Debug.Log("this part.");
                    
                } else if (enemyPathfinding != null) {
                    // enemyPathfinding.MoveTo(roamPosition);
                }

                StartCoroutine(AttackCooldownRoutine());
            }
        }

        private IEnumerator AttackCooldownRoutine() {
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }

        private Vector2 GetRoamingPosition() {
            timeRoaming = 0f;
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }
