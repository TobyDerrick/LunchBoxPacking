using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableObject : MonoBehaviour, IDraggable
{
    [Header("Draggable Settings")]
    public bool allowDragging = true;
    public bool useRigidbody = false;
    public bool snapToGrid = false;
    public float gridSize = 0.5f;

    private Vector3 dragOffset;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnBeginDrag(Vector3 worldPoint)
    {
        if (!allowDragging) return;
        dragOffset = transform.position - worldPoint;

        if (useRigidbody && rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void OnDrag(Vector3 worldPoint)
    {
        if (!allowDragging) return;

        Vector3 target = worldPoint + dragOffset;

        if (snapToGrid)
        {
            target.x = Mathf.Round(target.x / gridSize) * gridSize;
            target.z = Mathf.Round(target.z / gridSize) * gridSize;
        }

        if (useRigidbody && rb != null)
        {
            rb.MovePosition(target);
        }
        else
        {
            transform.position = target;
        }
    }

    public void OnEndDrag(Vector3 worldPoint)
    {
        if (!allowDragging) return;

        if (useRigidbody && rb != null)
        {
            rb.isKinematic = false;
        }
    }
}