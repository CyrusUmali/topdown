using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationParameters(Vector2 direction, bool isMoving, float attackSpeed)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("AttackSpeed", attackSpeed);
    }

    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }
}
