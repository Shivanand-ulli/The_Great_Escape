using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameScreenType
{
    MainGame,
    Pause,
    GameOver,
    None
}

public class MainGameUXController : MonoBehaviour
{
    [SerializeField] private string MetaSceneName;

    [Header("Header and Footer Panels")]
    [SerializeField] private GameObject headerPanel;
    [SerializeField] private GameObject footerPanel;

    [Header("Screen Manager Properties")]
    [SerializeField] private RectTransform screenContainer;
    [SerializeField] private bool includeInactiveScreens;
    [SerializeField] private bool autoDiscoverScreens;
    [SerializeField] private GameObject FTUEContainer;
    private GameScreenType startScreenType = GameScreenType.MainGame;

    public static MainGameUXController Instance { get; private set; }
    public GameObject FTUEContainerGO => FTUEContainer;

    private Dictionary<GameScreenType, GameBaseScreenUI> screenDict;
    private Stack<GameBaseScreenUI> screenHistory = new Stack<GameBaseScreenUI>();

    private bool isInitialized = false;
    private GameBaseScreenUI currentScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Start()
    {
        OnInitialize();
        ShowStartScreen();
        TryPlayGameMusic();
    }


    // --- UX Controller Functionality ---

    private void OnInitialize()
    {
        if (isInitialized)
        {
            return;
        }

        InitializeScreenDictonary();
        HideAllScreens();
        isInitialized = true;
    }

    private void InitializeScreenDictonary()
    {
        screenDict = new Dictionary<GameScreenType, GameBaseScreenUI>();

        if (autoDiscoverScreens)
        {
            AutoDiscoverScreens();
        }
        else
        {
            Debug.Log("Auto Discover screen is disabled, assing screens manually");
        }
    }

    private void AutoDiscoverScreens()
    {
        GameBaseScreenUI[] allScreens = screenContainer != null ? gameObject.GetComponentsInChildren<GameBaseScreenUI>(includeInactiveScreens) :
                                    FindObjectsByType<GameBaseScreenUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GameBaseScreenUI screen in allScreens)
        {
            GameScreenType screenType = screen.GetScreenType();
            if (screenDict.ContainsKey(screenType))
            {
                Debug.Log($"UxController containing duplicate screen type{screenType}.Overwriting with {screen.name}");
                continue;
            }

            screenDict[screenType] = screen;
            Debug.Log($"[UXController] Registered screen: {screenType} -> {screen.name}");
        }
    }

    private void ShowStartScreen()
    {
        if (screenDict == null || !screenDict.ContainsKey(startScreenType))
        {
            Debug.Log("Start screen is not registered in Dictonary");
            return;
        }

        SwitchScreen(startScreenType);
    }

    public IGameScreenUI SwitchScreen(GameScreenType screenType)
    {
        if (!isInitialized)
        {
            OnInitialize();
        }

        if (!screenDict.TryGetValue(screenType, out GameBaseScreenUI target) || target == null)
        {
            Debug.Log("Screen is not there in Dictonary");
        }

        return ShowScreenInternal(target, screenType, true);
    }

    private IGameScreenUI ShowScreenInternal(GameBaseScreenUI target, GameScreenType screenType, bool pushHistory)
    {
        if (currentScreen != null && currentScreen != target)
        {
            currentScreen.OnScreenHide();

            if (pushHistory)
            {
                screenHistory.Push(currentScreen);
            }
        }

        target.OnScreenInitialize();

        if (!target.gameObject.activeInHierarchy)
        {
            target.gameObject.SetActive(true);
            Debug.Log("Enabled gameobject of screen" + target);
        }

        target.OnScreenShow();

        currentScreen = target;
        return target;
    }

    private void HideAllScreens()
    {
        if (screenDict == null)
        {
            return;
        }

        foreach (GameBaseScreenUI screen in screenDict.Values)
        {
            if (screen == null)
            {
                continue;
            }

            if (screen.IsScreenVisible())
            {
                screen.OnScreenHide();
            }

            screen.SetScreenVisible(false);
        }

        screenHistory.Clear();
        currentScreen = null;

        Debug.Log("All screen hidden");
    }

    public void GoBack()
    {
        if (screenHistory == null || screenHistory.Count == 0)
        {
            return;
        }

        GameBaseScreenUI previousScreen = screenHistory.Pop();
        ShowScreenInternal(previousScreen, previousScreen.GetScreenType(), false);
    }

    // --- Header and Footer Functionality ---

    public void ShowHeader(bool show)
    {
        if (headerPanel != null)
        {
            headerPanel.SetActive(show);
        }
    }

    public void ShowFooter(bool show)
    {
        if (footerPanel != null)
        {
            footerPanel.SetActive(show);
        }
    }

    private void TryPlayGameMusic()
    {
        if (SoundManager.Instance != null)
        {
            // SoundManager.Instance.PlayMusic(SoundManager.Instance.gameBGMusicClip);
        }
    }
}
