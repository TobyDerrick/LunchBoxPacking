using UnityEngine;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance { get; private set; }

    [Header("References")]
    public Camera renderCamera; 
    public UnityEngine.UI.RawImage rtRawImage;

    [Header("Drag Plane")]
    public float planeHeight = 0f; 
    public Vector3 planeNormal = Vector3.up;

    [Header("Raycast")]
    public LayerMask draggableLayer = ~0;
    public float raycastDistance = 100f;

    private IDraggable currentDraggable;
    private GameObject currentGO;

    private Plane dragPlane;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        if (renderCamera == null) renderCamera = Camera.main;

        dragPlane = new Plane(planeNormal, new Vector3(0, planeHeight, 0));
    }

    public bool IsDragging => currentDraggable != null;

    public bool TryBeginDrag(Ray pickRay)
    {
        if (Physics.Raycast(pickRay, out RaycastHit hit, raycastDistance, draggableLayer, QueryTriggerInteraction.Ignore))
        {
            var draggable = hit.collider.GetComponentInParent<IDraggable>();
            if (draggable != null)
            {
                currentDraggable = draggable;
                currentGO = (hit.collider != null) ? hit.collider.gameObject : null;

                if (dragPlane.Raycast(pickRay, out float enter))
                {
                    Vector3 worldPoint = pickRay.GetPoint(enter);
                    currentDraggable.OnBeginDrag(worldPoint);
                }
                else
                {
                    currentDraggable.OnBeginDrag(hit.point);
                }

                return true;
            }
        }

        return false;
    }

    public void UpdateDrag(Ray pickRay)
    {
        if (currentDraggable == null) return;

        if (dragPlane.Raycast(pickRay, out float enter))
        {
            Vector3 worldPoint = pickRay.GetPoint(enter);
            currentDraggable.OnDrag(worldPoint);
        }
    }
    public void EndDrag()
    {
        if (currentDraggable == null) return;

        Vector3 lastPoint = new Vector3(0, planeHeight, 0);
        currentDraggable.OnEndDrag(lastPoint);

        currentDraggable = null;
        currentGO = null;
    }
    public Ray ScreenPointToCameraRay(Vector2 screenPoint)
    {
        if (rtRawImage != null)
        {
            RectTransform rt = rtRawImage.rectTransform;
            Canvas canvas = rt.GetComponentInParent<Canvas>();
            Camera uiCam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, uiCam, out Vector2 localPoint))
            {
                Vector2 size = rt.rect.size;
                Vector2 pivot = rt.pivot;

                Vector2 normalized = (localPoint + (size * 0.5f)) / size;
                Vector3 viewportPoint = new Vector3(normalized.x, normalized.y, 0f);

                if (renderCamera != null)
                    return renderCamera.ViewportPointToRay(viewportPoint);
            }
        }

        if (renderCamera != null)
            return renderCamera.ScreenPointToRay(screenPoint);

        return Camera.main.ScreenPointToRay(screenPoint);
    }
}