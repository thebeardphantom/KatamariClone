using UnityEngine;
using DG.Tweening;

public partial class Player : MonoBehaviour
{
    [Header("Camera")]
    public new Camera camera;
    [SerializeField]
    private float minDistance = -4f;
    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private Transform cameraSecondaryPivot;
    [SerializeField]
    private Transform minimapCamera;
    [SerializeField]
    private GameObject playerMask;
    private float yRotation;
    private bool isVisible = true;

    private void AwakeCamera()
    {
        yRotation = transform.eulerAngles.y;
        playerMask.transform.localScale = Vector3.zero;
    }
    private void LateUpdateCamera()
    {
        // Move camera to outer bounds of Katamari
        var secondaryPosition = cameraSecondaryPivot.localPosition;
        secondaryPosition.z = Mathf.SmoothStep(secondaryPosition.z, -(minDistance + katamariMaxRadius * 2f), Time.deltaTime * 2f);
        cameraSecondaryPivot.localPosition = secondaryPosition;

        // Store y-rotation separately to maintain non-rolled oriented camera pivot
        yRotation += GameInput.GetAxis(GameInput.ROTATE_ACTION) * 3f;
        cameraPivot.rotation = Quaternion.Euler(0f, yRotation, 0f);
        yRotation = cameraPivot.eulerAngles.y;
        var euler = cameraPivot.eulerAngles;
        euler.y = Mathf.LerpAngle(euler.y, cameraPivot.eulerAngles.y, 0.15f);
        cameraPivot.eulerAngles = euler;

        // Check for player visibility
        var isVisibleNow = false;
        var ray = camera.ViewportPointToRay(Vector3.one * 0.5f);
        var mask = LayerMask.GetMask("Player", "Default");
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, mask, QueryTriggerInteraction.Ignore))
        {
            isVisibleNow = hit.transform.IsChildOf(transform);
        }
        if (isVisible != isVisibleNow)
        {
            isVisible = isVisibleNow;
            DOTween.Kill(playerMask.transform);
            playerMask.transform.DOScale(isVisible ? 0f : 1f, 0.25f).SetEase(Ease.InOutQuad);
        }

        // Lock minimap camera rotation
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
