using UnityEngine;

public class MetaBaseScreen : MonoBehaviour, IMetaScreenUI
{
    [Header("Screen Configuration")]
    [SerializeField] protected MetaScreenType screenType = MetaScreenType.None;

    [Header("Screen Panel")]
    [SerializeField] protected GameObject screenPanel;

    protected bool isInitialized;
    protected bool isVisible;

    private Canvas canvas;

    public virtual void OnScreenInitialize()
    {
        if (isInitialized) return;

        isInitialized = true;
        GetOrCreateCanvas();
        OnScreenInitializeOverride();
    }

    public virtual void OnScreenShow()
    {
        isVisible = true;
        SetScreenVisible(true);
        RefreshScreen();
    }

    public virtual void OnScreenHide()
    {
        isVisible = false;
        SetScreenVisible(false);
    }

    public virtual MetaScreenType GetScreenType() => screenType;

    public virtual GameObject GetScreenGameObject() => screenPanel != null ? screenPanel : gameObject;

    public virtual bool IsScreenInitialized() => isInitialized;

    public virtual bool IsScreenVisible()
    {
        return isVisible && GetScreenGameObject().activeInHierarchy;
    }

    public virtual void SetScreenVisible(bool visible)
    {
        GetScreenGameObject().SetActive(visible);
    }

    /// <summary>Push latest data into the screen's widgets.</summary>
    public virtual void RefreshScreen() { }

    /// <summary>Higher order renders on top.</summary>
    public void SetSortingOrder(int order)
    {
        GetOrCreateCanvas().sortingOrder = order;
    }

    private Canvas GetOrCreateCanvas()
    {
        if (canvas == null) canvas = GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
        }
        return canvas;
    }

    protected virtual void OnScreenInitializeOverride() { }
}
