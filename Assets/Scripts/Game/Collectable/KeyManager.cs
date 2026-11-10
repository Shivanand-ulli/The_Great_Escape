using System;
using UnityEngine;

/// <summary>Per-level key state. Listeners (like the exit door) subscribe to OnKeyCollected.</summary>
public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public event Action OnKeyCollected;

    public bool IsKeyCollected { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void CollectKey()
    {
        if (IsKeyCollected) return;

        IsKeyCollected = true;
        OnKeyCollected?.Invoke();
    }
}
