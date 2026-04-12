using UnityEngine;

public abstract class UIPopup : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        
        UIManager.Instance.PopupStack.Push(this);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);

        UIManager.Instance.PopupStack.Pop();
    }
}
