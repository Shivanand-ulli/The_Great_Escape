using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads/saves PlayerData as a JSON file in Application.persistentDataPath.
/// Changes mark the data dirty; it is written on Save(), app pause and quit.
/// </summary>
public class PlayerDataManager : SingletonManager<PlayerDataManager>
{
    /// <summary>Fired after any change to the data.</summary>
    public event Action<PlayerData> OnDataChanged;

    [Header("Settings")]
    [SerializeField] private string fileName = "playerdata.json";
    [SerializeField] private bool prettyPrint = false;

    private PlayerData data;
    private string filePath;
    private bool isDirty;

    public PlayerData Data => data;

    /// <summary>Level being played, set by level select and kept up to date by LevelManager (1-based). 0 = none. Not saved to disk.</summary>
    public int CurrentLevel { get; set; }

    protected override void OnInitialize()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }

    protected override void OnCleanUp()
    {
        Save();
        OnDataChanged = null;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    // ---------- File IO ----------

    public void Load()
    {
        data = null;

        try
        {
            if (File.Exists(filePath))
            {
                data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(filePath));
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlayerDataManager] Failed to load save, using defaults. {e.Message}");
        }

        if (data == null)
        {
            data = new PlayerData();
            isDirty = true;
            Save();
        }
        else
        {
            isDirty = false;
        }

        OnDataChanged?.Invoke(data);
    }

    /// <summary>Writes to disk only if something changed.</summary>
    public void Save(bool force = false)
    {
        if (data == null || (!isDirty && !force)) return;

        try
        {
            // Write to a temp file first so a crash mid-write can't corrupt the save.
            string tempPath = filePath + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(data, prettyPrint));
            File.Copy(tempPath, filePath, true);
            File.Delete(tempPath);
            isDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Failed to save. {e.Message}");
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Failed to delete save. {e.Message}");
        }

        data = new PlayerData();
        isDirty = true;
        Save();
        OnDataChanged?.Invoke(data);
    }

    // ---------- Mutators ----------

    public void SetPlayerName(string playerName)
    {
        data.playerName = playerName;
        MarkChanged();
    }

    public void AddCoins(int amount)
    {
        data.coins = Mathf.Max(0, data.coins + amount);
        MarkChanged();
    }

    public void AddGems(int amount)
    {
        data.gems = Mathf.Max(0, data.gems + amount);
        MarkChanged();
    }

    public void AddKeys(int amount)
    {
        data.keys = Mathf.Max(0, data.keys + amount);
        MarkChanged();
    }

    public void SetVolumes(float music, float sfx)
    {
        data.musicVolume = Mathf.Clamp01(music);
        data.sfxVolume = Mathf.Clamp01(sfx);
        MarkChanged();
    }

    public void SetMusicMuted(bool muted)
    {
        if (data.musicMuted == muted) return;

        data.musicMuted = muted;
        MarkChanged();
    }

    public void SetSfxMuted(bool muted)
    {
        if (data.sfxMuted == muted) return;

        data.sfxMuted = muted;
        MarkChanged();
    }

    public void UnlockLevel(int levelIndex)
    {
        if (levelIndex <= data.highestUnlockedLevel) return;

        data.highestUnlockedLevel = levelIndex;
        MarkChanged();
    }

    /// <summary>Records a finished level, keeping the best score/time, and unlocks the next level.</summary>
    public void CompleteLevel(int levelIndex, int score, float time, int coinsCollected)
    {
        LevelProgress progress = data.GetOrCreateLevel(levelIndex);

        progress.completed = true;
        progress.bestScore = Mathf.Max(progress.bestScore, score);
        if (progress.bestTime <= 0f || (time > 0f && time < progress.bestTime)) progress.bestTime = time;
        progress.coinsCollected = Mathf.Max(progress.coinsCollected, coinsCollected);

        if (levelIndex + 1 > data.highestUnlockedLevel) data.highestUnlockedLevel = levelIndex + 1;

        MarkChanged();
        Save(); // level results are worth persisting immediately
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        LevelProgress progress = data.GetLevel(levelIndex);
        return progress != null && progress.completed;
    }

    private void MarkChanged()
    {
        isDirty = true;
        OnDataChanged?.Invoke(data);
    }
}
