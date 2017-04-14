using UnityEngine;
using System.Collections;
using UnityCommonLibrary.Utility;

public partial class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float groundedDistance = 0.05f;
    [SerializeField]
    private float gravity = 20f;
    private bool isGrounded;
    private new ConstantForce constantForce;

    private void AwakeMovement()
    {
        constantForce = gameObject.AddOrGetComponent<ConstantForce>();
    }
    private void UpdateMovement()
    {
        constantForce.force = Vector3.down * gravity * rigidbody.mass;
        isGrounded = false;
        // Find ground
        var ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            isGrounded = hit.distance < groundedDistance;
        }
        var movement = new Vector3(GameInput.GetAxis(GameInput.MOVE_ACTION), 0f, -GameInput.GetAxis(GameInput.STRAFE_ACTION));
        rigidbody.angularVelocity = cameraPivot.TransformDirection(movement) * moveSpeed;
    }
}