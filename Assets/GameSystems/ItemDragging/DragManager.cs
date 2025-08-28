using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [SerializeField] private CursorController cursorController;

    [Header("Input Actions")]
    public InputActionReference pointerPosition;
    public InputActionReference pointerPress;
    public InputActionReference rotateAction;

    [Header("Drag Settings")]
    public float dragStrength = 50f;
    public float dragDamping = 5f;
    public float maxForce = 1000f;              // Clamp linear force
    public float maxAngularForce = 500f;        // Clamp rotational torque

    [Header("Lift Settings")]
    public float maxLiftHeight = 2f;
    public float liftSmoothTime = 0.1f;         // SmoothDamp lift
    private float liftVelocity = 0f;

    [Header("Rotation Settings")]
    public Vector3 desiredRotation = Vector3.zero;
    public float rotationStabilizeSpeed = 5f;
    public float rotationDamping = 5f;
    public float rotationAdjustSpeed = 90f;

    [Header("Layers")]
    public LayerMask foodLayer;

    private Camera cam;
    private Rigidbody heldRb;
    private float liftAmount = 0f;

    private void OnEnable()
    {
        pointerPosition.action.Enable();
        pointerPress.action.Enable();

        pointerPress.action.performed += ctx => TryPickObject();
        pointerPress.action.canceled += ctx => ReleaseObject();

        cam = Camera.main;
    }

    private void OnDisable()
    {
        pointerPosition.action.Disable();
        pointerPress.action.Disable();
    }

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

        if (heldRb == null) return;

        Quaternion rotationDelta = Quaternion.Euler(input.y * rotAmount, input.x * rotAmount, 0f);
        Quaternion currentRotation = Quaternion.Euler(desiredRotation);
        Quaternion newRotation = rotationDelta * currentRotation;

        desiredRotation = newRotation.eulerAngles;
    }

    private void UpdateCursorHover()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, foodLayer))
        {
            cursorController.SetCursorState(heldRb == null ? CursorState.Hover : CursorState.Grab);
        }
        else if (heldRb == null)
        {
            cursorController.SetCursorState(CursorState.Default);
        }
    }

    private void TryPickObject()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, foodLayer))
        {
            if (hit.rigidbody == null) return;

            cursorController.SetCursorState(CursorState.Grab);
            heldRb = hit.rigidbody;

            desiredRotation = heldRb.transform.rotation.eulerAngles;

            heldRb.useGravity = false;
            heldRb.interpolation = RigidbodyInterpolation.Interpolate;
            heldRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            liftAmount = 0f;

            if (heldRb.transform.parent != null && heldRb.transform.parent.CompareTag("Plate"))
                heldRb.transform.SetParent(null);
        }
    }

    private void ReleaseObject()
    {
        if (heldRb != null)
        {
            cursorController.SetCursorState(CursorState.Default);

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

        // Smooth lift
        liftAmount = Mathf.SmoothDamp(liftAmount, maxLiftHeight, ref liftVelocity, liftSmoothTime);
        targetPoint.y += liftAmount;

        // Compute desired velocity and apply mass-scaled force
        Vector3 toTarget = targetPoint - heldRb.position;
        Vector3 desiredVelocity = toTarget * dragStrength;
        Vector3 force = (desiredVelocity - heldRb.linearVelocity) * heldRb.mass;

        // Clamp force to prevent extreme spikes
        force = Vector3.ClampMagnitude(force, maxForce);

        heldRb.AddForce(force - heldRb.linearVelocity * dragDamping, ForceMode.Force);
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

            // Clamp torque
            Vector3 angularDelta = torque - heldRb.angularVelocity * rotationDamping;
            angularDelta = Vector3.ClampMagnitude(angularDelta, maxAngularForce);

            heldRb.AddTorque(angularDelta, ForceMode.Acceleration);
        }
    }
}
