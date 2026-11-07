using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform rectTransform;

    [Header("Panel Configuration")]
    [Tooltip("The panel RectTransform that animates. If not assigned, will automatically find a child named 'Panel' or fallback to root.")]
    [SerializeField] private RectTransform panel;

    [Header("Background Configuration")]
    public bool isShowBg = true;
    [Tooltip("Tag of the background in PopupDatabase to use for this popup")]
    [SerializeField] private string backgroundTag;

    protected GameObject background;
    protected Image backgroundImage;

    [Header("Animation Settings")]
    [SerializeField] private bool useCloseAnimation = false;

    // Animation parameters
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector3 hiddenScale = new(0.8f, 0.8f, 1f);
    [SerializeField] private Vector3 shownScale = Vector3.one;
    private bool isAnimating = false;
    public Action OnClosed;

    public RectTransform Panel
    {
        get => panel;
        set => panel = value;
    }

    public string BackgroundTag
    {
        get => backgroundTag;
        set => backgroundTag = value;
    }

    public GameObject Background => background;
    public Image BackgroundImage => backgroundImage;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Ensure root rectTransform stays at normal scale so background does not animate
        rectTransform.localScale = Vector3.one;

        FindPanel();

        if (panel != null && panel != rectTransform)
        {
            panel.localScale = hiddenScale;
        }
        else
        {
            rectTransform.localScale = hiddenScale;
        }
    }

    private void FindPanel()
    {
        if (panel != null) return;

        // 1. Try to find direct child named "Panel" (case-insensitive)
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (string.Equals(child.name, "Panel", StringComparison.OrdinalIgnoreCase))
            {
                panel = child.GetComponent<RectTransform>();
                if (panel != null) return;
            }
        }

        // 2. Try to find any child containing "Panel"
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.IndexOf("Panel", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                panel = child.GetComponent<RectTransform>();
                if (panel != null) return;
            }
        }

        // 3. Fallback: first child that isn't named "BG" or "PopupBackground"
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string childName = child.name.ToLower();
            if (!childName.Contains("bg") && !childName.Contains("background"))
            {
                panel = child.GetComponent<RectTransform>();
                if (panel != null) return;
            }
        }

        // 4. Fallback to root
        panel = rectTransform;
    }

    public virtual void Init(Canvas parentCanvas, int sortingOrder)
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;

        // Set world camera if needed
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            canvas.worldCamera = parentCanvas.worldCamera;

        transform.SetParent(parentCanvas.transform, false);

        // Ensure panel is resolved
        if (panel == null || panel == rectTransform)
        {
            FindPanel();
        }

        // Setup background
        SetupBackgroundInstance();

        // Initialize hook
        OnInit();

        // Play the opening animation
        ShowAnimated();
        PlayOpenSound();
    }

    protected virtual void SetupBackgroundInstance()
    {
        if (!isShowBg)
        {
            if (background != null)
                background.SetActive(false);

            Transform directBg = transform.Find("BG");
            if (directBg != null) directBg.gameObject.SetActive(false);

            return;
        }

        if (background == null)
        {
            // Fetch background prefab by tag from PopupDatabase via PopupManager
            GameObject prefab = null;
            if (PopupManager.Instance != null)
            {
                prefab = PopupManager.Instance.GetPopupBackgroundPrefab(backgroundTag);
            }

            if (prefab != null)
            {
                background = Instantiate(prefab, transform, false);
                background.name = "PopupBackground";
                background.transform.SetAsFirstSibling();
                backgroundImage = background.GetComponent<Image>();

                RectTransform bgRect = background.GetComponent<RectTransform>();
                if (bgRect != null)
                {
                    bgRect.anchorMin = Vector2.zero;
                    bgRect.anchorMax = Vector2.one;
                    bgRect.offsetMin = Vector2.zero;
                    bgRect.offsetMax = Vector2.zero;
                    bgRect.localScale = Vector3.one;
                }
            }
            else
            {
                // Fallback: check if the popup prefab already contains a child named "BG" or "PopupBackground"
                Transform existingBg = transform.Find("BG") ?? transform.Find("PopupBackground");
                if (existingBg != null)
                {
                    background = existingBg.gameObject;
                    backgroundImage = background.GetComponent<Image>();
                }
            }
        }

        // Hide any duplicate direct child "BG" if a separate background was instantiated
        Transform duplicateBg = transform.Find("BG");
        if (duplicateBg != null && duplicateBg.gameObject != background)
        {
            duplicateBg.gameObject.SetActive(false);
        }

        if (background != null)
        {
            background.SetActive(true);
            SetupBackground();
        }
    }

    protected virtual void SetupBackground()
    {
        // base popup uses default background
    }

    /// <summary>
    /// Called after initialization, before animation plays.
    /// Popups can override this to wire up buttons.
    /// </summary>
    protected virtual void OnInit()
    {
        // Base implementation does nothing
    }

    private void ShowAnimated()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateShow());
    }

    private void CloseAnimated()
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (isAnimating) return;
        StartCoroutine(AnimateClose());
    }

    private IEnumerator AnimateShow()
    {
        if (isAnimating) yield break;

        isAnimating = true;

        Transform targetTransform = (panel != null && panel != rectTransform) ? panel : rectTransform;

        targetTransform.localScale = hiddenScale;

        float timeElapsed = 0f;
        Vector3 initialScale = hiddenScale;
        float initialAlpha = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);
            
            // Ease Out Cubic
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            
            targetTransform.localScale = Vector3.LerpUnclamped(initialScale, shownScale, easeT);
            
            yield return null;
        }

        targetTransform.localScale = shownScale;
        isAnimating = false;
    }

    private IEnumerator AnimateClose()
    {
        if (!gameObject.activeInHierarchy) yield break;
        if (isAnimating) yield break;

        isAnimating = true;

        Transform targetTransform = (panel != null && panel != rectTransform) ? panel : rectTransform;

        float timeElapsed = 0f;
        Vector3 initialScale = targetTransform.localScale;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);
            
            // Ease In Cubic
            float easeT = t * t * t;
            
            targetTransform.localScale = Vector3.LerpUnclamped(initialScale, hiddenScale, easeT);
            
            yield return null;
        }

        targetTransform.localScale = hiddenScale;
        
        isAnimating = false;
        OnCloseComplete();
    }

    protected virtual void OnCloseComplete()
    {
        if (background != null && background.transform.parent != transform)
        {
            Destroy(background);
            background = null;
        }
        OnClosed?.Invoke();

        Destroy(gameObject);
    }

    public virtual void Close()
    {
        if (useCloseAnimation)
            CloseAnimated();
        else
            OnCloseComplete();
    }

    /// <summary>
    /// Called when the GameObject is being destroyed.
    /// Notifies PopupManager to remove this popup from the active list.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (background != null && background.transform.parent != transform)
        {
            Destroy(background);
            background = null;
        }

        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.OnPopupDestroyed(this);
        }
    }
    
    protected virtual void PlayOpenSound()
    {
        // Play sound if you have a SoundManager
        // if (SoundManager.Instance != null)
        // {
        //     SoundManager.Instance.Play(SoundSFX.PopupOpenClip);
        // }
    }
}

