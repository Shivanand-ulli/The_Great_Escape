using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Wraps the Poki SDK. Created automatically when the game starts (no scene setup needed).
///  - Initializes the SDK as early as possible.
///  - Tells Poki when the game finished loading (GameLoadingFinished).
///  - Sends gameplayStart / gameplayStop by following CoreGameManager's state.
///  - CommercialBreak / RewardedBreak mute the audio and freeze the game while an ad runs.
/// Poki only exists in WebGL builds and in the editor (where the SDK just logs), so on any other
/// platform every call simply runs the callback straight away.
/// </summary>
public class PokiManager : SingletonManager<PokiManager>
{
    public static bool IsAvailable => Application.isEditor || Application.platform == RuntimePlatform.WebGLPlayer;

    private bool isGameplayActive;
    private bool isAdRunning;
    private bool loadingFinishedSent;
    private float cachedTimeScale = 1f;
    private Action commercialDone;
    private Action<bool> rewardedDone;
    private CoreGameManager hookedGame;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        if (!IsAvailable || Instance != null) return;

        new GameObject("PokiManager").AddComponent<PokiManager>();
    }

    protected override void OnInitialize()
    {
        if (!IsAvailable) return;

        // The SDK throws if init is called twice, so guard it.
        try
        {
            PokiUnitySDK.Instance.init();
            PokiUnitySDK.Instance.gameLoadingStart();
        }
        catch (PokiException e)
        {
            Debug.LogWarning($"[PokiManager] {e.Message}");
        }

        StartCoroutine(HookGameState());
    }

    protected override void OnCleanUp()
    {
        if (hookedGame != null) hookedGame.OnGameStateChanged -= HandleGameStateChanged;
        hookedGame = null;
    }

    // CoreGameManager is created by the bootstrap, so wait for it instead of assuming it exists.
    private IEnumerator HookGameState()
    {
        while (CoreGameManager.Instance == null) yield return null;

        hookedGame = CoreGameManager.Instance;
        hookedGame.OnGameStateChanged += HandleGameStateChanged;
    }

    // ---------- Loading ----------

    /// <summary>Call once, when the game is loaded and the player can start. Safe to call more than once.</summary>
    public static void GameLoadingFinished()
    {
        if (!IsAvailable || Instance == null || Instance.loadingFinishedSent) return;

        Instance.loadingFinishedSent = true;
        PokiUnitySDK.Instance.gameLoadingFinished();
    }

    // ---------- Gameplay start / stop ----------

    // Playing = gameplay. Paused, Won, Lost or None = not gameplay.
    private void HandleGameStateChanged(GameState previous, GameState current)
    {
        if (current == GameState.Playing) GameplayStart();
        else GameplayStop();
    }

    private void GameplayStart()
    {
        if (isGameplayActive || isAdRunning) return;

        isGameplayActive = true;
        PokiUnitySDK.Instance.gameplayStart();
    }

    private void GameplayStop()
    {
        if (!isGameplayActive) return;

        isGameplayActive = false;
        PokiUnitySDK.Instance.gameplayStop();
    }

    // ---------- Ads ----------

    /// <summary>
    /// Call at a natural break, just before gameplay would start again (next level, restart, level
    /// select). Poki decides if an ad actually plays. onComplete is always called afterwards.
    /// </summary>
    public static void CommercialBreak(Action onComplete)
    {
        if (!IsAvailable || Instance == null || Instance.isAdRunning)
        {
            onComplete?.Invoke();
            return;
        }

        Instance.StartCommercialBreak(onComplete);
    }

    /// <summary>Rewarded ad. onComplete gets true if the player should receive the reward.</summary>
    public static void RewardedBreak(Action<bool> onComplete)
    {
        if (!IsAvailable || Instance == null || Instance.isAdRunning)
        {
            onComplete?.Invoke(false);
            return;
        }

        Instance.StartRewardedBreak(onComplete);
    }

    private void StartCommercialBreak(Action onComplete)
    {
        BeginAd();
        commercialDone = onComplete;

        PokiUnitySDK.Instance.commercialBreakCallBack = OnCommercialBreakFinished;
        PokiUnitySDK.Instance.commercialBreak();
    }

    private void StartRewardedBreak(Action<bool> onComplete)
    {
        BeginAd();
        rewardedDone = onComplete;

        PokiUnitySDK.Instance.rewardedBreakCallBack = OnRewardedBreakFinished;
        PokiUnitySDK.Instance.rewardedBreak();
    }

    private void OnCommercialBreakFinished()
    {
        EndAd();

        Action done = commercialDone;
        commercialDone = null;
        done?.Invoke();
    }

    private void OnRewardedBreakFinished(bool withReward)
    {
        EndAd();

        Action<bool> done = rewardedDone;
        rewardedDone = null;
        done?.Invoke(withReward);
    }

    // Poki requires audio and input to be off while an ad plays.
    private void BeginAd()
    {
        isAdRunning = true;
        GameplayStop();

        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (SoundManager.Instance != null) SoundManager.Instance.PauseAudio();
    }

    private void EndAd()
    {
        isAdRunning = false;

        Time.timeScale = cachedTimeScale;

        if (SoundManager.Instance != null) SoundManager.Instance.ResumeAudio();
    }
}
