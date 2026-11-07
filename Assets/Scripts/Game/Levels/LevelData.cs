using System;
using UnityEngine;

/// <summary>One level entry. Added directly in the LevelDataCollection list (no separate asset needed).</summary>
[Serializable]
public class LevelData
{
    [Tooltip("Unique id, e.g. level_01. Used to find the level without relying on its position.")]
    [SerializeField] private string levelId;
    [SerializeField] private string levelName;
    [Tooltip("Turn off to disable this level without removing it from the list.")]
    [SerializeField] private bool isActive = true;
    [SerializeField] private GameObject levelPrefab;

    public string LevelId => levelId;
    public string LevelName => levelName;
    public bool IsActive => isActive;
    public GameObject LevelPrefab => levelPrefab;
}
