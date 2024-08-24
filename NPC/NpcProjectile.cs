using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;
    [SerializeField] private float offsetDistance = 5f;

    private Vector3 startPosition;
    private Vector3 direction;
    private int damageAmount;
    private string instantiatorCollider; // Field to store the instantiator's collider

    private void Start()
    {
        startPosition = transform.position + direction.normalized * offsetDistance;
        transform.position = startPosition;
        RotateProjectile();
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
        RotateProjectile();
    }

    private void RotateProjectile()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public void SetDamage(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    public void SetInstantiatorCollider(string collider)
    {
        instantiatorCollider = collider;
    }

  private void OnTriggerEnter2D(Collider2D other)
{

    // Debug.Log("instantiatorCollider:"+instantiatorCollider);
    if (other.name == instantiatorCollider)
    {
        // Debug.Log("Ignoring collision with instantiator's collider: " + other.name);
        return;
    }

    if (other is BoxCollider2D && other.isTrigger)
    {
        NpcHealth npcHealth = other.gameObject.GetComponent<NpcHealth>();

        if (npcHealth != null)
        {
            // Debug.Log("Arrow hit " + other.name);
            npcHealth.TakeDamage(damageAmount , gameObject);
            Destroy(gameObject);
        }
    }
}


    private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}

