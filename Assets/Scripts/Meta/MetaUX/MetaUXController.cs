using System.Collections.Generic;
using UnityEngine;

public enum MetaScreenType
{
    Home,
    LevelSelect,
    Settings,
    None
}

/// <summary>Controls the Meta (menu) UI: which screen is shown, plus header and footer.</summary>
public class MetaUXController : MonoBehaviour
{
    [Header("Header and Footer")]
    [SerializeField] private MetaHeaderController headerController;
    [SerializeField] private MetaFooterController footerController;

    [Header("Screens")]
    [SerializeField] private MetaScreenType startScreenType = MetaScreenType.Home;
    [SerializeField] private RectTransform screenContainer;
    [SerializeField] private bool includeInactiveScreens = true;
    [Tooltip("Find screens automatically. If off, fill the Screens list.")]
    [SerializeField] private bool autoDiscoverScreens = true;
    [SerializeField] private List<MetaBaseScreenUI> screens = new List<MetaBaseScreenUI>();

    public static MetaUXController Instance { get; private set; }
    public MetaScreenType CurrentScreenType => currentScreen != null ? currentScreen.GetScreenType() : MetaScreenType.None;

    private readonly Dictionary<MetaScreenType, MetaBaseScreenUI> screenDict = new Dictionary<MetaScreenType, MetaBaseScreenUI>();
    private readonly Stack<MetaBaseScreenUI> screenHistory = new Stack<MetaBaseScreenUI>();

    private MetaBaseScreenUI currentScreen;
    private bool isInitialized;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        Initialize();
        SwitchScreen(startScreenType);
    }

    private void Initialize()
    {
        if (isInitialized) return;

        RegisterScreens();
        HideAllScreens();
        isInitialized = true;
    }

    private void RegisterScreens()
    {
        screenDict.Clear();

        if (autoDiscoverScreens)
        {
            screens.Clear();
            if (screenContainer != null)
            {
                screens.AddRange(screenContainer.GetComponentsInChildren<MetaBaseScreenUI>(includeInactiveScreens));
            }
            else
            {
                screens.AddRange(FindObjectsByType<MetaBaseScreenUI>(
                    includeInactiveScreens ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None));
            }
        }

        for (int i = 0; i < screens.Count; i++)
        {
            MetaBaseScreenUI screen = screens[i];
            if (screen == null) continue;

            MetaScreenType type = screen.GetScreenType();
            if (!screenDict.TryAdd(type, screen))
            {
                Debug.LogWarning($"[MetaUX] Duplicate screen type {type} on '{screen.name}' ignored.");
            }
        }
    }

    // ---------- Screens ----------

    public IMetaScreenUI SwitchScreen(MetaScreenType screenType)
    {
        Initialize();

        if (!screenDict.TryGetValue(screenType, out MetaBaseScreenUI target) || target == null)
        {
            Debug.LogWarning($"[MetaUX] Screen {screenType} is not registered.");
            return null;
        }

        return ShowScreen(target, true);
    }

    public void GoBack()
    {
        while (screenHistory.Count > 0)
        {
            MetaBaseScreenUI previous = screenHistory.Pop();
            if (previous == null) continue;

            ShowScreen(previous, false);
            return;
        }
    }

    public T GetScreen<T>(MetaScreenType screenType) where T : MetaBaseScreenUI
    {
        return screenDict.TryGetValue(screenType, out MetaBaseScreenUI screen) ? screen as T : null;
    }

    public void RefreshCurrentScreen()
    {
        if (currentScreen != null) currentScreen.RefreshScreen();
    }

    private IMetaScreenUI ShowScreen(MetaBaseScreenUI target, bool pushHistory)
    {
        if (currentScreen == target) return target;

        if (currentScreen != null)
        {
            currentScreen.OnScreenHide();
            if (pushHistory) screenHistory.Push(currentScreen);
        }

        target.OnScreenInitialize();
        target.OnScreenShow();
        currentScreen = target;
        return target;
    }

    private void HideAllScreens()
    {
        foreach (MetaBaseScreenUI screen in screenDict.Values)
        {
            if (screen == null) continue;

            if (screen.IsScreenVisible()) screen.OnScreenHide();
            screen.SetScreenVisible(false);
        }

        screenHistory.Clear();
        currentScreen = null;
    }

    // ---------- Header and Footer ----------

    public void ShowHeader(bool show)
    {
        if (headerController != null) headerController.SetVisible(show);
    }

    public void ShowFooter(bool show)
    {
        if (footerController != null) footerController.SetVisible(show);
    }

    public void RefreshHeader()
    {
        if (headerController != null) headerController.RefreshHeader();
    }
}
