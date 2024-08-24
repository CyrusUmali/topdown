using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour, IEnemy
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 2f;
    private bool isJumping = false;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = PlayerController.Instance.transform;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Slime");
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player transform not found. Make sure PlayerController.Instance is set correctly.");
        }
    }

    public void Attack()
    {
        if (!isJumping)
        {
            // Debug.Log("Starting JumpTowardsPlayer coroutine");
            // StartCoroutine(JumpTowardsPlayer());
        }
        else
        {
            // Debug.Log("Already jumping, skipping Attack");
        }
    }

    private IEnumerator JumpTowardsPlayer()
    {
        isJumping = true;

        if (playerTransform == null)
        {
            Debug.LogError("Player transform is null. Exiting JumpTowardsPlayer coroutine.");
            yield break;
        }

        Vector2 jumpDirection = (playerTransform.position - transform.position).normalized;
        Debug.Log($"Jump direction: {jumpDirection}");

        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        Debug.Log($"Applied force: {jumpDirection * jumpForce}");

        yield return new WaitForSeconds(jumpCooldown);
        Debug.Log("Jump cooldown finished");

        isJumping = false;
    }
}
