using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}