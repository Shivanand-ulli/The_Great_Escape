using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionPopup : PopupBase
{
    [Header("Data References")]
    [SerializeField] private LevelDataCollection levelDataCollection;

    [Header("UI References")]
    [SerializeField] private RectTransform levelGridContainer;
    [SerializeField] private LevelButtonUI levelButtonPrefab;
    [SerializeField] private Button CloseButton;

    [Header("Pagination")]
    [Tooltip("How many level buttons fit in the grid at once.")]
    [SerializeField, Min(1)] private int levelsPerPage = 20;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    [Header("Pagination Dots")]
    [SerializeField] private RectTransform dotsContainer;
    [SerializeField] private Image dotPrefab;
    [SerializeField] private Sprite activeDotSprite;
    [SerializeField] private Sprite inactiveDotSprite;

    [Header("Scene")]
    [Tooltip("Scene that contains LevelManager. Must be in Build Settings.")]
    [SerializeField] private string gameSceneName = "Game";

    /// <summary>Fired with the 1-based level number when a level button is pressed.</summary>
    public event Action<int> OnLevelSelected;

    private readonly List<LevelButtonUI> buttonPool = new List<LevelButtonUI>();
    private readonly List<Image> dots = new List<Image>();

    private PlayerData playerData;
    private int totalLevels;
    private int totalPages;
    private int currentPage;
    private bool isLoadingGame;

    private void OnEnable()
    {
        CloseButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDisable()
    {
        CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    protected override void OnInit()
    {
        if (levelDataCollection == null || levelButtonPrefab == null || levelGridContainer == null)
        {
            Debug.LogError("[LevelSelectionPopup] Assign LevelDataCollection, Level Grid Container and Level Button Prefab in the inspector.", this);
            return;
        }

        playerData = PlayerDataManager.Instance != null ? PlayerDataManager.Instance.Data : null;
        totalLevels = levelDataCollection.Count;
        totalPages = Mathf.Max(1, Mathf.CeilToInt((float)totalLevels / levelsPerPage));

        if (nextPageButton != null) nextPageButton.onClick.AddListener(NextPage);
        if (prevPageButton != null) prevPageButton.onClick.AddListener(PrevPage);

        CreateButtonPool();
        CreateDots();

        // Open on the page that contains the level the player should play next.
        int currentLevel = playerData != null ? playerData.highestUnlockedLevel : 1;
        currentPage = Mathf.Clamp((currentLevel - 1) / levelsPerPage, 0, totalPages - 1);

        RefreshPage();
    }

    protected override void OnDestroy()
    {
        if (nextPageButton != null) nextPageButton.onClick.RemoveListener(NextPage);
        if (prevPageButton != null) prevPageButton.onClick.RemoveListener(PrevPage);

        base.OnDestroy();
    }

    // Only one page worth of buttons is created, then reused for every page.
    private void CreateButtonPool()
    {
        int poolSize = Mathf.Min(levelsPerPage, totalLevels);
        for (int i = 0; i < poolSize; i++)
        {
            LevelButtonUI button = Instantiate(levelButtonPrefab, levelGridContainer);
            button.gameObject.SetActive(false);
            buttonPool.Add(button);
        }
    }

    private void CreateDots()
    {
        if (dotsContainer == null || dotPrefab == null) return;

        for (int i = 0; i < totalPages; i++)
        {
            dots.Add(Instantiate(dotPrefab, dotsContainer));
        }
    }

    private void RefreshPage()
    {
        int startIndex = currentPage * levelsPerPage;

        for (int i = 0; i < buttonPool.Count; i++)
        {
            int levelNumber = startIndex + i + 1;
            LevelButtonUI button = buttonPool[i];

            if (levelNumber > totalLevels)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            button.Setup(levelNumber, GetStatus(levelNumber), HandleLevelSelected);
        }

        // Arrows only appear when there is a page in that direction.
        if (prevPageButton != null) prevPageButton.gameObject.SetActive(currentPage > 0);
        if (nextPageButton != null) nextPageButton.gameObject.SetActive(currentPage < totalPages - 1);

        // Dots only matter when there is more than one page.
        for (int i = 0; i < dots.Count; i++)
        {
            Image dot = dots[i];
            dot.gameObject.SetActive(totalPages > 1);

            Sprite sprite = i == currentPage ? activeDotSprite : inactiveDotSprite;
            if (sprite != null)
            {
                dot.sprite = sprite;
                dot.SetNativeSize();
            }
        }
    }

    private void NextPage()
    {
        if (currentPage >= totalPages - 1) return;

        currentPage++;
        RefreshPage();
    }

    private void PrevPage()
    {
        if (currentPage <= 0) return;

        currentPage--;
        RefreshPage();
    }

    // Locked: level is switched off in the collection, or the player hasn't reached it.
    // Completed: player has finished it. Active: unlocked and not finished yet.
    private LevelStatus GetStatus(int levelNumber)
    {
        if (!levelDataCollection.IsLevelActive(levelNumber)) return LevelStatus.Locked;

        int highestUnlocked = playerData != null ? playerData.highestUnlockedLevel : 1;
        if (levelNumber > highestUnlocked) return LevelStatus.Locked;

        LevelProgress progress = playerData != null ? playerData.GetLevel(levelNumber) : null;
        return progress != null && progress.completed ? LevelStatus.Completed : LevelStatus.Active;
    }

    // Remember which level was picked, then open the game scene. LevelManager starts that level there.
    private void HandleLevelSelected(int levelNumber)
    {
        if (isLoadingGame) return;
        isLoadingGame = true;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CurrentLevel = levelNumber;
        }

        OnLevelSelected?.Invoke(levelNumber);

        // Natural break right before gameplay: Poki may show an ad, then the game scene opens.
        PokiManager.CommercialBreak(() =>
        {
            Close();
            SceneManager.LoadScene(gameSceneName);
        });
    }
    
    private void OnCloseButtonClicked()
    {
        Close();
    }
}
