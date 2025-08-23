using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class RegionTrigger : MonoBehaviour
{
    [Header("Event Signals")]
    public UnityEvent<GameObject> OnObjectEnter;
    public UnityEvent<GameObject> OnObjectExit;

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
            OnObjectEnter?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    { 

        if(IsValidLayer(other.gameObject))
        { 
            OnObjectExit?.Invoke(other.gameObject);
        }
    }

    private bool IsValidLayer(GameObject obj)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }
}
