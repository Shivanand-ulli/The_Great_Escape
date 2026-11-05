using System;
using UnityEngine;

public enum GameState
{
    None,
    Playing,
    Paused,
    Won,
    Lost
}

/// <summary>
/// Owns the global game state (playing / paused / won / lost) and time scale.
/// Self-contained: other systems subscribe to the events below.
/// </summary>
public class CoreGameManager : SingletonManager<CoreGameManager>
{
    /// <summary>Fired on every state change: (previous, current).</summary>
    public event Action<GameState, GameState> OnGameStateChanged;
    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameWon;
    public event Action OnGameLost;

    [Header("Settings")]
    [SerializeField] private bool pauseAudioOnPause = true;

    private GameState currentState = GameState.None;
    private float cachedTimeScale = 1f;

    public GameState CurrentState => currentState;
    public bool IsPlaying => currentState == GameState.Playing;
    public bool IsPaused => currentState == GameState.Paused;
    public bool IsGameOver => currentState == GameState.Won || currentState == GameState.Lost;

    protected override void OnInitialize()
    {
        ResetState();
    }

    protected override void OnCleanUp()
    {
        // Never leave the game frozen when this manager goes away.
        Time.timeScale = 1f;
        SetPauseAudio(false);

        OnGameStateChanged = null;
        OnGameStarted = null;
        OnGamePaused = null;
        OnGameResumed = null;
        OnGameWon = null;
        OnGameLost = null;
    }

    /// <summary>Begin (or restart) a level.</summary>
    public void StartGame()
    {
        ApplyTimeScale(1f);
        SetPauseAudio(false);
        if (SetState(GameState.Playing))
        {
            OnGameStarted?.Invoke();
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;

        cachedTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
        ApplyTimeScale(0f);
        SetPauseAudio(true);

        SetState(GameState.Paused);
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;

        ApplyTimeScale(cachedTimeScale);
        SetPauseAudio(false);

        SetState(GameState.Playing);
        OnGameResumed?.Invoke();
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing) PauseGame();
        else if (currentState == GameState.Paused) ResumeGame();
    }

    public void WinGame()
    {
        if (!CanEnd()) return;

        EndGame(GameState.Won);
        OnGameWon?.Invoke();
    }

    public void LoseGame()
    {
        if (!CanEnd()) return;

        EndGame(GameState.Lost);
        OnGameLost?.Invoke();
    }

    /// <summary>Back to None and normal time (call before leaving a level).</summary>
    public void ResetState()
    {
        ApplyTimeScale(1f);
        SetPauseAudio(false);
        cachedTimeScale = 1f;

        SetState(GameState.None);
    }

    // Win/lose only valid while actively playing; first result wins.
    private bool CanEnd() => currentState == GameState.Playing;

    private void EndGame(GameState result)
    {
        SetPauseAudio(false);
        SetState(result);
    }

    private bool SetState(GameState newState)
    {
        if (currentState == newState) return false;

        GameState previous = currentState;
        currentState = newState;
        OnGameStateChanged?.Invoke(previous, newState);
        return true;
    }

    private static void ApplyTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    private void SetPauseAudio(bool paused)
    {
        if (!pauseAudioOnPause) return;

        SoundManager sound = SoundManager.Instance;
        if (sound == null) return;

        if (paused) sound.PauseAudio();
        else sound.ResumeAudio();
    }
}
