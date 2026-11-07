using UnityEngine;

public class GameBaseScreen : MonoBehaviour,IGameScreenUI
{
    [Header("Screen Configuration")]
    [SerializeField] protected GameScreenType screenType = GameScreenType.None;
    [Tooltip("Optional: The UI area where the player is allowed to drag and drop items")]
    public RectTransform inputAreaUI;

    [Header("Screen Panel")]
    [SerializeField] protected GameObject screenPanel;

    [Header("Screen State")]
    protected bool isInitialized = false;
    protected bool isVisible = false;

    private Canvas canvas;

    public virtual void OnScreenInitialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        GetOrCreateCanvas();

        OnScreenInitializeOverride();
    }

    private Canvas GetOrCreateCanvas()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
        }
        return canvas;
    }

    public virtual void OnScreenShow()
    {
        isVisible = true;
        SetScreenVisible(true);

        // if (inputAreaUI != null && Player.Instance != null)
        // {
        //     Player.Instance.SetInputArea(inputAreaUI);
        // }
    }

    public virtual void OnScreenHide()
    {
        isVisible = false;
        SetScreenVisible(false);
    }

    public virtual GameScreenType GetScreenType()
    {
        return screenType;
    }

    public virtual GameObject GetScreenGameObject()
    {
        return screenPanel;
    }

    public virtual bool IsScreenInitialized()
    {
        return isInitialized;
    }

    public virtual bool IsScreenVisible()
    {
        if (screenPanel != null)
        {
            return isVisible && screenPanel.activeInHierarchy;
        }

        return isVisible && gameObject.activeInHierarchy;
    }

    public virtual void SetScreenVisible(bool visible)
    {
        if (screenPanel != null)
        {
            screenPanel.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    public virtual void RefreshScreen()
    {

    }

    /// <summary>
    /// Sets the Canvas sorting order for this screen
    /// </summary>
    /// <param name="order">The sorting order value (higher = renders on top)</param>
    public void SetSortingOrder(int order)
    {
        Canvas screenCanvas = GetOrCreateCanvas();
        if (screenCanvas != null)
        {
            screenCanvas.sortingOrder = order;
        }
    }

    protected virtual void OnScreenInitializeOverride() { }
}
