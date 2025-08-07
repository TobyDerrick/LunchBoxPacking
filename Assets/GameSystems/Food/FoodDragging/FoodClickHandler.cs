using UnityEngine;
using UnityEngine.InputSystem;

public class FoodClickHandler : MonoBehaviour
{
    [SerializeField] private LayerMask foodLayer;

    private Camera mainCam;
    private GameInputActions inputActions;

    private void Awake()
    {
        mainCam = Camera.main;
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Debug.Log($"Mouse position: {mousePos}");

        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Debug.Log($"Ray origin: {ray.origin}, direction: {ray.direction}");

        if (Physics.Raycast(ray, out hit, 100f, foodLayer))
        {
            Debug.Log("Raycast hit");
            var draggable = hit.collider.GetComponentInParent<FoodDraggable>();
            if (draggable != null)
            {
                draggable.BeginDrag();
            }
        }
        else
        {
            Debug.Log("Raycast missed");
        }
    }
}
