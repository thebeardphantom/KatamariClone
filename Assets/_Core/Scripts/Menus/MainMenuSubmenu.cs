using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class MainMenuSubmenu : MonoBehaviour
{
    private static int IS_ACTIVE_HASH = Animator.StringToHash("isActive");

    private Animator animator;
    private CanvasGroup canvasGroup;

    public virtual IEnumerator EnterState()
    {
        animator.SetBool(IS_ACTIVE_HASH, true);
        canvasGroup.alpha = 1f;
        yield return null;
        // Wait for animation to finish
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    public virtual IEnumerator ExitState()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        animator.SetBool(IS_ACTIVE_HASH, false);
        yield return null;
        // Wait for animation to finish
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        canvasGroup.alpha = 0f;
    }
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    protected virtual void Start()
    {
        animator.SetBool(IS_ACTIVE_HASH, false);
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
