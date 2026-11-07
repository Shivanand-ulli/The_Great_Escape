using UnityEngine;

public class MetaFooterController : MonoBehaviour
{
    [Header("Footer root")]
    [SerializeField] protected GameObject footerRoot;

    public void RefreshFooter()
    {
        RefreshFooterOverride();
    }

    public void SetVisible(bool visible)
    {
        (footerRoot != null ? footerRoot : gameObject).SetActive(visible);
    }

    public virtual GameObject GetFooterGameObject() => gameObject;

    protected virtual void RefreshFooterOverride() { }
}
