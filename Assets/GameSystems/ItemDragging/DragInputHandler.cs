using UnityEngine;
using UnityEngine.InputSystem;

public class DragInputHandler : MonoBehaviour
{
    public InputActionReference pointerPositionAction;
    public InputActionReference clickAction;

    private Vector2 currentPointer;
    private bool isPointerDown = false;

    void OnEnable()
    {
        if (pointerPositionAction != null) pointerPositionAction.action.Enable();
        if (clickAction != null)
        {
            clickAction.action.Enable();
            clickAction.action.started += OnClickStarted;
            clickAction.action.canceled += OnClickCanceled;
        }
    }

    void OnDisable()
    {
        if (pointerPositionAction != null) pointerPositionAction.action.Disable();
        if (clickAction != null)
        {
            clickAction.action.started -= OnClickStarted;
            clickAction.action.canceled -= OnClickCanceled;
            clickAction.action.Disable();
        }
    }

    void Update()
    {
        if (pointerPositionAction != null)
        {
            currentPointer = pointerPositionAction.action.ReadValue<Vector2>();
        }

        if (isPointerDown && DragManager.Instance != null)
        {
            Ray r = DragManager.Instance.ScreenPointToCameraRay(currentPointer);
            DragManager.Instance.UpdateDrag(r);
        }
    }

    void OnClickStarted(InputAction.CallbackContext ctx)
    {
        isPointerDown = true;

        if (DragManager.Instance == null) return;

        Ray r = DragManager.Instance.ScreenPointToCameraRay(currentPointer);
        bool started = DragManager.Instance.TryBeginDrag(r);

    }

    void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        isPointerDown = false;

        if (DragManager.Instance == null) return;

        DragManager.Instance.EndDrag();
    }
}
