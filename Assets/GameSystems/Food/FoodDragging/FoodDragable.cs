using UnityEngine;
using UnityEngine.InputSystem;

public class FoodDraggable : MonoBehaviour
{
    [SerializeField] private Transform origin;
    private bool isDragging;
    private Vector3 offset;
    private Camera cam;
    private float dragZ;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void BeginDrag()
    {
        isDragging = true;

        dragZ = cam.WorldToScreenPoint(origin.position).z;

        Vector3 mouseWorldPos = GetMouseWorldPos();
        offset = origin.position - mouseWorldPos;
    }

    private void Update()
    {
        if (isDragging)
        {
            origin.position = GetMouseWorldPos() + offset;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndDrag();
            }
        }
    }

    private void EndDrag()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = dragZ;
        return cam.ScreenToWorldPoint(screenPos);
    }
}
