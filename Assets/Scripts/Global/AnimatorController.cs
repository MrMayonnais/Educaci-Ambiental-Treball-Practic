using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.enabled = true;
    }

    public void DisableAnimator()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
    
    
}
