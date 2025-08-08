using UnityEngine;

public interface IDraggable
{
    /// <summary>
    /// Called when dragging begins. worldPoint is the point on the drag plane where the pointer hit.
    /// </summary>
    void OnBeginDrag(Vector3 worldPoint);

    /// <summary>
    /// Called while dragging. worldPoint is the point on the drag plane beneath the pointer.
    /// </summary>
    void OnDrag(Vector3 worldPoint);

    /// <summary>
    /// Called when dragging ends. worldPoint is the last known point on the drag plane.
    /// </summary>
    void OnEndDrag(Vector3 worldPoint);
}