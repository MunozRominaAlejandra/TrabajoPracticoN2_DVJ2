using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack(int tipoAtaque, bool isAttacking)
    {
        animator.SetInteger("Attack", tipoAtaque);
        animator.SetBool("isAttacking", isAttacking);
    }

    public void TerminarAtaque()
    {
        animator.SetBool("isAttacking", false);
    }

    public void Defender(bool defendiendo)
    {
        animator.SetBool("isDefended", defendiendo);
    }
}