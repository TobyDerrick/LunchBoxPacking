using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class RegionTrigger : MonoBehaviour
{

    [Header("Events")]
    public UnityEvent<GameObject> OnObjectEnterEvent;
    public UnityEvent<GameObject> OnObjectExitEvent;

    [Header("Filter Settings")]
    [Tooltip("Only objects on these layers will trigger events")]
    public LayerMask layerMask;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidLayer(other.gameObject))
        {
            OnObjectEnterEvent?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    { 
        if(IsValidLayer(other.gameObject))
        { 
            OnObjectExitEvent?.Invoke(other.gameObject);
        }
    }

    private bool IsValidLayer(GameObject obj)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }
}
