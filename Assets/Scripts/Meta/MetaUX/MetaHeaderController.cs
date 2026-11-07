using UnityEngine;

public class MetaHeaderController : MonoBehaviour
{
    [Header("Header root")]
    [SerializeField] protected GameObject headerRoot;

    public void RefreshHeader()
    {
        RefreshHeaderOverride();
    }

    public void SetVisible(bool visible)
    {
        (headerRoot != null ? headerRoot : gameObject).SetActive(visible);
    }

    public virtual GameObject GetHeaderGameObject() => gameObject;

    /// <summary>Push latest data into already-bound widgets.</summary>
    protected virtual void RefreshHeaderOverride() { }
}
