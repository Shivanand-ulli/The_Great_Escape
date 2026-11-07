using System;
using System.Collections.Generic;

/// <summary>Plain serializable save data. Stored as JSON by PlayerDataManager.</summary>
[Serializable]
public class PlayerData
{
    public int version = 1;

    public string playerName = "Player";

    // Currency / collectables
    public int coins;
    public int gems;
    public int keys;

    // Progression
    public int highestUnlockedLevel = 1;
    public List<LevelProgress> levels = new List<LevelProgress>();

    // Settings
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicMuted;
    public bool sfxMuted;

    public LevelProgress GetLevel(int levelIndex)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].levelIndex == levelIndex) return levels[i];
        }
        return null;
    }

    public LevelProgress GetOrCreateLevel(int levelIndex)
    {
        LevelProgress progress = GetLevel(levelIndex);
        if (progress == null)
        {
            progress = new LevelProgress { levelIndex = levelIndex };
            levels.Add(progress);
        }
        return progress;
    }
}

[Serializable]
public class LevelProgress
{
    public int levelIndex;
    public bool completed;
    public int bestScore;
    public float bestTime;   // 0 = not set
    public int coinsCollected;
}
