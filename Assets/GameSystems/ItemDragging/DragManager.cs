using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CursorController cursorController;

    [Header("Input Actions")]
    public InputActionReference pointerPosition;
    public InputActionReference pointerPress;
    public InputActionReference rotateAction;

    [Header("Drag Settings")]
    public float dragStrength = 50f;
    public float dragDamping = 5f;
    public float maxForce = 1000f;
    public float maxAngularForce = 500f;

    [Header("Lift Settings")]
    public float maxLiftHeight = 2f;
    public float liftSmoothTime = 0.1f;
    private float liftVelocity = 0f;
    private float liftAmount = 0f;

    [Header("Rotation Settings")]
    public Vector3 desiredRotation = Vector3.zero;
    public float rotationStabilizeSpeed = 5f;
    public float rotationDamping = 5f;
    public float rotationAdjustSpeed = 90f;

    [Header("Layers")]
    public LayerMask foodLayer;

    private Camera cam;
    private Rigidbody heldRb;
    private Vector3 grabLocalOffset;

    private void OnEnable()
    {
        pointerPosition.action.Enable();
        pointerPress.action.Enable();
        rotateAction.action.Enable();

        pointerPress.action.performed += OnPressPerformed;
        pointerPress.action.canceled += OnPressCanceled;

        cam = Camera.main;
    }

    private void OnDisable()
    {
        pointerPress.action.performed -= OnPressPerformed;
        pointerPress.action.canceled -= OnPressCanceled;

        pointerPosition.action.Disable();
        pointerPress.action.Disable();
        rotateAction.action.Disable();
    }

    private void OnPressPerformed(InputAction.CallbackContext ctx) => TryPickObject();
    private void OnPressCanceled(InputAction.CallbackContext ctx) => ReleaseObject();

    private void Update()
    {
        if (heldRb != null)
            HandleRotationInput();

        UpdateCursorHover();
    }

    private void FixedUpdate()
    {
        if (heldRb != null)
        {
            UpdateDragPhysics();
            UpdateRotationPhysics();
        }
    }

    private void HandleRotationInput()
    {
        Vector2 input = rotateAction.action.ReadValue<Vector2>();
        float rotAmount = rotationAdjustSpeed * Time.deltaTime;

        Quaternion rotationDelta = Quaternion.Euler(input.y * rotAmount, input.x * rotAmount, 0f);
        Quaternion currentRotation = Quaternion.Euler(desiredRotation);
        desiredRotation = (rotationDelta * currentRotation).eulerAngles;
    }

    private void UpdateCursorHover()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, foodLayer))
            cursorController.SetCursorState(heldRb == null ? CursorState.Hover : CursorState.Grab);
        else if (heldRb == null)
            cursorController.SetCursorState(CursorState.Default);
    }

    private void TryPickObject()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, foodLayer))
        {
            if (hit.rigidbody == null) return;

            heldRb = hit.rigidbody;
            cursorController.SetCursorState(CursorState.Grab);

            desiredRotation = heldRb.transform.rotation.eulerAngles;

            heldRb.useGravity = false;
            heldRb.interpolation = RigidbodyInterpolation.Interpolate;
            heldRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            liftAmount = 0f;
            grabLocalOffset = heldRb.transform.InverseTransformPoint(hit.point);

            if (heldRb.transform.parent != null && heldRb.transform.parent.CompareTag("Plate"))
                heldRb.transform.SetParent(null);
        }
    }

    private void ReleaseObject()
    {
        if (heldRb != null)
        {
            cursorController.SetCursorState(CursorState.Default);
            heldRb.angularVelocity = Vector3.zero;
            heldRb.useGravity = true;
            heldRb = null;
        }
    }

    private void UpdateDragPhysics()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (!plane.Raycast(ray, out float dist)) return;

        Vector3 targetPoint = ray.GetPoint(dist);
        liftAmount = Mathf.SmoothDamp(liftAmount, maxLiftHeight, ref liftVelocity, liftSmoothTime);
        targetPoint.y += liftAmount;

        Vector3 worldGrabPoint = heldRb.transform.TransformPoint(grabLocalOffset);
        Vector3 force = (targetPoint - worldGrabPoint) * dragStrength - heldRb.GetPointVelocity(worldGrabPoint) * dragDamping;
        force = Vector3.ClampMagnitude(force, maxForce);

        heldRb.AddForceAtPosition(force, worldGrabPoint, ForceMode.Acceleration);
    }

    private void UpdateRotationPhysics()
    {
        Quaternion targetRot = Quaternion.Euler(desiredRotation);
        Quaternion delta = targetRot * Quaternion.Inverse(heldRb.rotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        if (Mathf.Abs(angle) > 0.01f)
        {
            Vector3 torque = axis * angle * rotationStabilizeSpeed * heldRb.mass;
            Vector3 angularDelta = torque - heldRb.angularVelocity * rotationDamping;
            angularDelta = Vector3.ClampMagnitude(angularDelta, maxAngularForce);
            heldRb.AddTorque(angularDelta, ForceMode.Acceleration);
        }
    }
}
