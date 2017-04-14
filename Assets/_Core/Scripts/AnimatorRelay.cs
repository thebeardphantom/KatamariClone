using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimatorRelay : MonoBehaviour
{
    public delegate void OnAnimatorIKDelegate(int layerIndex);
    public event OnAnimatorIKDelegate AnimatorIK;

    public Animator animator { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (AnimatorIK != null)
        {
            AnimatorIK(layerIndex);
        }
    }
}
