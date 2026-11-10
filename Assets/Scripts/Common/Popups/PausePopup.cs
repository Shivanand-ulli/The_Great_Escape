using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePopup : PopupBase
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    [Header("Music")]
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;

    [Header("SFX")]
    [SerializeField] private Button sfxOnButton;
    [SerializeField] private Button sfxOffButton;

    protected override void OnInit()
    {
        AddListener(continueButton, OnContinueClicked);
        AddListener(restartButton, OnRestartClicked);
        AddListener(homeButton, OnHomeClicked);

        AddListener(musicOnButton, MuteMusic);
        AddListener(musicOffButton, UnmuteMusic);
        AddListener(sfxOnButton, MuteSfx);
        AddListener(sfxOffButton, UnmuteSfx);

        SoundManager sound = SoundManager.Instance;
        UpdateMusicButtonState(sound != null && sound.IsMusicMuted);
        UpdateSFXButtonState(sound != null && sound.IsSfxMuted);

        // Opening this popup pauses the game.
        if (CoreGameManager.Instance != null) CoreGameManager.Instance.PauseGame();
    }

    protected override void OnDestroy()
    {
        RemoveListener(continueButton, OnContinueClicked);
        RemoveListener(restartButton, OnRestartClicked);
        RemoveListener(homeButton, OnHomeClicked);

        RemoveListener(musicOnButton, MuteMusic);
        RemoveListener(musicOffButton, UnmuteMusic);
        RemoveListener(sfxOnButton, MuteSfx);
        RemoveListener(sfxOffButton, UnmuteSfx);

        // Safety: however the popup goes away, never leave the game paused behind it.
        if (CoreGameManager.Instance != null && CoreGameManager.Instance.IsPaused)
        {
            CoreGameManager.Instance.ResumeGame();
        }

        base.OnDestroy();
    }

    // ---------- Main buttons ----------

    private void OnContinueClicked()
    {
        PlayClick();
        if (CoreGameManager.Instance != null) CoreGameManager.Instance.ResumeGame();
        Close();
    }

    private void OnRestartClicked()
    {
        PlayClick();

        // StartLevel resets time scale and audio, so the game runs normally again.
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
        else if (CoreGameManager.Instance != null)
        {
            CoreGameManager.Instance.ResumeGame();
        }

        Close();
    }

    private void OnHomeClicked()
    {
        PlayClick();
        PopupManager.Instance.ShowPopup<ConformationPopup>();
    }

    // ---------- Music / SFX ----------

    // The "On" button is visible while sound is on, so pressing it mutes.
    // The "Off" button is visible while muted, so pressing it unmutes.
    private void MuteMusic() => SetMusicMuted(true);

    private void UnmuteMusic() => SetMusicMuted(false);

    private void MuteSfx() => SetSfxMuted(true);

    private void UnmuteSfx()
    {
        SetSfxMuted(false);
        PlayClick(); // plays only now that SFX is unmuted
    }

    private void SetMusicMuted(bool muted)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicMuted(muted);
        UpdateMusicButtonState(muted);
    }

    private void SetSfxMuted(bool muted)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSfxMuted(muted);
        UpdateSFXButtonState(muted);
    }

    private void UpdateMusicButtonState(bool isMuted)
    {
        if (musicOnButton != null) musicOnButton.gameObject.SetActive(!isMuted);
        if (musicOffButton != null) musicOffButton.gameObject.SetActive(isMuted);
    }

    private void UpdateSFXButtonState(bool isMuted)
    {
        if (sfxOnButton != null) sfxOnButton.gameObject.SetActive(!isMuted);
        if (sfxOffButton != null) sfxOffButton.gameObject.SetActive(isMuted);
    }

    // ---------- Helpers ----------

    private static void PlayClick()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
    }

    private static void AddListener(Button button, UnityAction action)
    {
        if (button != null) button.onClick.AddListener(action);
    }

    private static void RemoveListener(Button button, UnityAction action)
    {
        if (button != null) button.onClick.RemoveListener(action);
    }
}
