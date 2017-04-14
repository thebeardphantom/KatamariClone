using UnityEngine;
using System.Collections;

public partial class Player : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private GameObject ethan;
    [SerializeField]
    private Vector3 ethanOffset;
    [SerializeField]
    private Vector3 ethanHandsOffset;
    private AnimatorRelay ethanAnimatorRelay;

    private void AwakeAnimation()
    {
        ethanAnimatorRelay = ethan.GetComponent<AnimatorRelay>();
        ethan.transform.SetParent(null, true);
    }
    private void EnableAnimation()
    {
        ethanAnimatorRelay.AnimatorIK += OnEthanIK;
    }
    private void DisableAnimation()
    {
        ethanAnimatorRelay.animator.speed = 0f;
        ethanAnimatorRelay.AnimatorIK -= OnEthanIK;
    }
    private void OnEthanIK(int layerIndex)
    {
        var animator = ethanAnimatorRelay.animator;
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);

        // Determine right-hand offset from body
        var localOffset = ethanHandsOffset;
        var globalOffset = ethan.transform.TransformVector(localOffset);
        animator.SetIKPosition(AvatarIKGoal.RightHand, ethan.transform.position + globalOffset);
        // Reverse offset on local-x for left hand
        localOffset.x *= -1f;
        globalOffset = ethan.transform.TransformVector(localOffset);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, ethan.transform.position + globalOffset);
    }
    private void LateUpdateAnimation()
    {
        // Up-projected forward ray
        var ray = new Ray(cameraPivot.position + Vector3.up * sphere.radius, cameraPivot.forward);
        // Start point pushed backwards
        var finalEthanPosition = ray.GetPoint(-katamariAverageRadius + ethanOffset.z);
        RaycastHit hit;
        if (Physics.Raycast(finalEthanPosition, Vector3.down, out hit, sphere.radius * 5f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(finalEthanPosition, hit.point);
            finalEthanPosition.y = hit.point.y;
        }
        else
        {
            // Place Ethan at Katamari's collider ground level
            finalEthanPosition = finalEthanPosition + Vector3.down * sphere.radius;
        }
        ethan.transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
        ethan.transform.position = finalEthanPosition;

        // Send values to animator
        var animator = ethanAnimatorRelay.animator;
        var currentValue = animator.GetFloat("Forward");
        var moveInput = GameInput.GetAxis(GameInput.MOVE_ACTION);
        animator.SetFloat("Forward", Mathf.Lerp(currentValue, Mathf.Abs(moveInput), 0.25f));
        currentValue = animator.GetFloat("Turn");
        animator.SetFloat("Turn", Mathf.Lerp(currentValue, GameInput.GetAxis(GameInput.ROTATE_ACTION) * Mathf.Sign(moveInput), 0.25f));
    }
}