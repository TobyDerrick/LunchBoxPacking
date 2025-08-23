using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [SerializeField] private CursorController cursorController;

    [Header("Input Actions")]
    public InputActionReference pointerPosition; 
    public InputActionReference pointerPress;    

    [Header("Drag Settings")]
    public float dragStrength = 50f;
    public float dragDamping = 5f;
    public float maxDistance = 0.1f;

    [Header("Lift Settings")]
    public float maxLiftHeight = 2f;
    public float liftSpeed = 2f;

    [Header("Layers")]
    public LayerMask foodLayer;

    private Camera cam;
    private SpringJoint joint;
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
        if (joint != null)
        {
            UpdateDragPoint();
        }
        UpdateCursorHover();
    }

    private void UpdateCursorHover()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, foodLayer))
        {
            if (heldRb == null)
            {
                cursorController.SetCursorState(CursorState.Hover);
            }
        }
        else
        {
            if (heldRb == null)
            {
                cursorController.SetCursorState(CursorState.Default);
            }
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

            joint = heldRb.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = hit.point;
            joint.minDistance = 0.01f;
            joint.maxDistance = maxDistance;
            joint.spring = dragStrength;
            joint.damper = dragDamping;

            heldRb.useGravity = false;

            liftAmount = 0f;

            heldRb.transform.SetParent(null);
        }
    }

    private void ReleaseObject()
    {
        if (joint != null)
        {
            cursorController.SetCursorState(CursorState.Default);
            Destroy(joint);
            heldRb.useGravity = true;
            heldRb = null;
        }
    }

    private void UpdateDragPoint()
    {
        Vector2 screenPos = pointerPosition.action.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(screenPos);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float dist))
        {
            Vector3 point = ray.GetPoint(dist);

            liftAmount = Mathf.MoveTowards(liftAmount, maxLiftHeight, liftSpeed * Time.deltaTime);
            point.y += liftAmount;

            joint.connectedAnchor = point;
        }
    }
}
