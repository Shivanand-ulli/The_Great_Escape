using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The list of all levels. Add entries with the + button in the inspector and drop each level prefab in.
/// Level 1 is the first entry, level 2 the second, and so on, so no numbering to get wrong.
/// </summary>
[CreateAssetMenu(fileName = "LevelDataCollection", menuName = "The Great Escape/Level Data Collection")]
public class LevelDataCollection : ScriptableObject
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();

    public IReadOnlyList<LevelData> Levels => levels;
    public int Count => levels.Count;

    /// <summary>Level numbers start at 1.</summary>
    public bool HasLevel(int levelNumber)
    {
        return levelNumber >= 1 && levelNumber <= levels.Count;
    }

    /// <summary>Returns the level data for this number (1-based), or null if it doesn't exist.</summary>
    public LevelData GetLevel(int levelNumber)
    {
        return HasLevel(levelNumber) ? levels[levelNumber - 1] : null;
    }

    public GameObject GetLevelPrefab(int levelNumber)
    {
        LevelData data = GetLevel(levelNumber);
        return data != null ? data.LevelPrefab : null;
    }

    /// <summary>Display name; falls back to "Level N" when no name is set.</summary>
    public string GetLevelName(int levelNumber)
    {
        LevelData data = GetLevel(levelNumber);
        return data != null && !string.IsNullOrEmpty(data.LevelName) ? data.LevelName : $"Level {levelNumber}";
    }

    public bool HasNextLevel(int levelNumber)
    {
        return HasLevel(levelNumber + 1);
    }

    /// <summary>True if the level exists and its Is Active box is ticked.</summary>
    public bool IsLevelActive(int levelNumber)
    {
        LevelData data = GetLevel(levelNumber);
        return data != null && data.IsActive;
    }

    /// <summary>Finds a level by its Level Id. Returns null if not found.</summary>
    public LevelData GetLevelById(string levelId)
    {
        return TryGetLevelNumber(levelId, out int number) ? levels[number - 1] : null;
    }

    /// <summary>Gets the 1-based level number for a Level Id.</summary>
    public bool TryGetLevelNumber(string levelId, out int levelNumber)
    {
        EnsureIdLookup();
        return idToNumber.TryGetValue(levelId ?? string.Empty, out levelNumber);
    }

    /// <summary>Next level number after this one that is active, or 0 if there is none.</summary>
    public int GetNextActiveLevelNumber(int levelNumber)
    {
        for (int next = Mathf.Max(levelNumber + 1, 1); next <= levels.Count; next++)
        {
            if (IsLevelActive(next)) return next;
        }
        return 0;
    }

    // ---------- Id lookup (built once, rebuilt after inspector edits) ----------

    private Dictionary<string, int> idToNumber;

    private void EnsureIdLookup()
    {
        if (idToNumber != null) return;

        idToNumber = new Dictionary<string, int>(levels.Count);
        for (int i = 0; i < levels.Count; i++)
        {
            LevelData data = levels[i];
            if (data == null || string.IsNullOrEmpty(data.LevelId)) continue;

            if (!idToNumber.TryAdd(data.LevelId, i + 1))
            {
                Debug.LogWarning($"[LevelDataCollection] Duplicate level id '{data.LevelId}' at level {i + 1} ignored.", this);
            }
        }
    }

    private void OnEnable() => idToNumber = null;
    private void OnValidate() => idToNumber = null;
}
