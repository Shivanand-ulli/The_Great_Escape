using System;
using UnityEngine;

/// <summary>
/// Controls the level flow in the game scene: spawn a level prefab, complete it, fail it,
/// restart it or move on to the next one. Win/lose state goes through CoreGameManager,
/// progress is saved through PlayerDataManager.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDataCollection levelDataCollection;

    [Header("Spawn")]
    [Tooltip("Parent for the spawned level prefab. Leave empty to spawn at the scene root.")]
    [SerializeField] private Transform levelParent;

    [Header("Start")]
    [Tooltip("Start a level automatically when the scene begins.")]
    [SerializeField] private bool startOnStart = true;
    [Tooltip("Used only if PlayerDataManager has no current level and no saved progress.")]
    [SerializeField, Min(1)] private int fallbackLevel = 1;

    // Static so UI that exists before LevelManager.Awake can subscribe safely (LevelManager.Instance
    // can still be null at that point). Always unsubscribe in OnDisable/OnDestroy.
    public static event Action<int> OnLevelStarted;
    public static event Action<int> OnLevelCompleted;
    public static event Action<int> OnLevelFailed;
    /// <summary>Fired when LoadNextLevel is called after the last active level.</summary>
    public static event Action OnAllLevelsCompleted;

    private GameObject currentLevelInstance;
    private int currentLevelNumber;
    private float levelTime;
    private bool isLevelRunning;

    public int CurrentLevelNumber => currentLevelNumber;
    public float LevelTime => levelTime;
    public bool IsLevelRunning => isLevelRunning;
    public bool HasNextLevel => levelDataCollection != null && levelDataCollection.GetNextActiveLevelNumber(currentLevelNumber) > 0;

    /// <summary>The LevelManager of the current game scene.</summary>
    public static LevelManager Instance { get; private set; }

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
        if (startOnStart) StartLevel(GetInitialLevel());
    }

    private void Update()
    {
        // deltaTime is 0 while the game is paused, so the timer pauses with it.
        if (isLevelRunning) levelTime += Time.deltaTime;
    }

    /// <summary>Spawns the level prefab and starts the game. Returns false if the level can't be started.</summary>
    public bool StartLevel(int levelNumber)
    {
        if (levelDataCollection == null)
        {
            Debug.LogError("[LevelManager] LevelDataCollection is not assigned.", this);
            return false;
        }

        if (!levelDataCollection.IsLevelActive(levelNumber))
        {
            Debug.LogWarning($"[LevelManager] Level {levelNumber} doesn't exist or is not active.", this);
            return false;
        }

        GameObject prefab = levelDataCollection.GetLevelPrefab(levelNumber);
        if (prefab == null)
        {
            Debug.LogWarning($"[LevelManager] Level {levelNumber} has no prefab assigned.", this);
            return false;
        }

        DestroyCurrentLevel();

        currentLevelInstance = Instantiate(prefab, levelParent);
        currentLevelNumber = levelNumber;
        levelTime = 0f;
        isLevelRunning = true;

        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.CurrentLevel = levelNumber;

        if (CoreGameManager.Instance != null) CoreGameManager.Instance.StartGame();

        OnLevelStarted?.Invoke(levelNumber);
        return true;
    }

    public bool RestartLevel()
    {
        return StartLevel(currentLevelNumber);
    }

    /// <summary>Player finished the level. Saves progress and sets the game to Won.</summary>
    public void LevelCompleted(int score = 0, int coinsCollected = 0)
    {
        if (!isLevelRunning) return;

        isLevelRunning = false;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CompleteLevel(currentLevelNumber, score, levelTime, coinsCollected);
        }

        if (CoreGameManager.Instance != null) CoreGameManager.Instance.WinGame();

        OnLevelCompleted?.Invoke(currentLevelNumber);
    }

    /// <summary>Player lost the level. Sets the game to Lost.</summary>
    public void LevelFailed()
    {
        if (!isLevelRunning) return;

        isLevelRunning = false;

        if (CoreGameManager.Instance != null) CoreGameManager.Instance.LoseGame();

        OnLevelFailed?.Invoke(currentLevelNumber);
    }

    /// <summary>Starts the next active level. Fires OnAllLevelsCompleted if there is none.</summary>
    public bool LoadNextLevel()
    {
        int next = levelDataCollection != null ? levelDataCollection.GetNextActiveLevelNumber(currentLevelNumber) : 0;
        if (next <= 0)
        {
            OnAllLevelsCompleted?.Invoke();
            return false;
        }

        // If an inactive level was skipped, make sure the level we jump to is unlocked.
        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.UnlockLevel(next);

        return StartLevel(next);
    }

    private int GetInitialLevel()
    {
        PlayerDataManager data = PlayerDataManager.Instance;
        if (data == null) return fallbackLevel;

        // Level the player picked in level select.
        if (data.CurrentLevel > 0) return data.CurrentLevel;
        if (data.Data != null) return data.Data.highestUnlockedLevel;
        return fallbackLevel;
    }

    private void DestroyCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }
    }
}
