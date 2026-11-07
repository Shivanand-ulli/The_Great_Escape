using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container holding a popup background prefab mapped to a unique tag.
/// </summary>
[Serializable]
public class PopupBackgroundData
{
    [Tooltip("Unique tag or identifier for this background (e.g., 'Default', 'Dark', 'Blur', etc.)")]
    public string tag;

    [Tooltip("Background prefab to instantiate for this tag")]
    public GameObject backgroundPrefab;
}

[CreateAssetMenu(fileName = "SO_PopupDatabase", menuName = "Popup/Popup Database")]
public class PopupDatabase : ScriptableObject
{
    [Header("Popup Prefabs")]
    public List<GameObject> popupPrefabs = new List<GameObject>();

    [Header("Popup Backgrounds")]
    [Tooltip("List of available popup backgrounds mapped to tags")]
    public List<PopupBackgroundData> popupBackgrounds = new List<PopupBackgroundData>();

    /// <summary>
    /// Finds the background prefab associated with the given tag.
    /// If tag is empty or not found, falls back to an entry tagged 'Default' or the first available background.
    /// </summary>
    /// <param name="tag">Tag to search for</param>
    /// <returns>Matching background prefab or fallback, or null if none available</returns>
    public GameObject GetBackgroundPrefab(string tag)
    {
        if (popupBackgrounds == null || popupBackgrounds.Count == 0)
            return null;

        // 1. Search for matching tag (case-insensitive)
        if (!string.IsNullOrEmpty(tag))
        {
            for (int i = 0; i < popupBackgrounds.Count; i++)
            {
                var bg = popupBackgrounds[i];
                if (bg != null && bg.backgroundPrefab != null &&
                    string.Equals(bg.tag, tag, StringComparison.OrdinalIgnoreCase))
                {
                    return bg.backgroundPrefab;
                }
            }

            Debug.LogWarning($"[PopupDatabase] No popup background found with tag '{tag}'. Falling back to default.");
        }

        // 2. Search for entry with tag 'Default'
        for (int i = 0; i < popupBackgrounds.Count; i++)
        {
            var bg = popupBackgrounds[i];
            if (bg != null && bg.backgroundPrefab != null &&
                string.Equals(bg.tag, "Default", StringComparison.OrdinalIgnoreCase))
            {
                return bg.backgroundPrefab;
            }
        }

        // 3. Fallback to first non-null entry
        for (int i = 0; i < popupBackgrounds.Count; i++)
        {
            var bg = popupBackgrounds[i];
            if (bg != null && bg.backgroundPrefab != null)
            {
                return bg.backgroundPrefab;
            }
        }

        return null;
    }
}

