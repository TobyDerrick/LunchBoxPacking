using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public virtual void Initialize()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deinitialize()
    {
        gameObject.SetActive(false);
    }
}
