using UnityEngine;

public class GameHeaderController : MonoBehaviour
{
    [Header("Header root")]
    [SerializeField] protected GameObject headerRoot;

    public void RefreshHeader()
    {
        RefreshHeaderOverride();
    }

    public void SetVisible(bool visible)
    {
        if (headerRoot != null)
        {
            headerRoot.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    public virtual GameObject GetHeaderGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// Push latest data into already-bound widgets (call <see cref="RefreshHeader"/> after economy changes).
    /// </summary>
    protected virtual void RefreshHeaderOverride() { }
}
