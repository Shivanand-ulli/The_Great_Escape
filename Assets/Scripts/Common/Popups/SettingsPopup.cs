using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase
{
    [Header("Music")]
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;

    [Header("SFX")]
    [SerializeField] private Button sfxOnButton;
    [SerializeField] private Button sfxOffButton;

    [Header("Close")]
    [SerializeField] private Button closeButton;

    protected override void OnInit()
    {
        AddListener(musicOnButton, MuteMusic);
        AddListener(musicOffButton, UnmuteMusic);
        AddListener(sfxOnButton, MuteSfx);
        AddListener(sfxOffButton, UnmuteSfx);
        AddListener(closeButton, Close);

        SoundManager sound = SoundManager.Instance;
        UpdateMusicButtonState(sound != null && sound.IsMusicMuted);
        UpdateSFXButtonState(sound != null && sound.IsSfxMuted);
    }

    protected override void OnDestroy()
    {
        RemoveListener(musicOnButton, MuteMusic);
        RemoveListener(musicOffButton, UnmuteMusic);
        RemoveListener(sfxOnButton, MuteSfx);
        RemoveListener(sfxOffButton, UnmuteSfx);
        RemoveListener(closeButton, Close);

        base.OnDestroy();
    }

    // The "On" button is visible while sound is on, so pressing it mutes.
    // The "Off" button is visible while muted, so pressing it unmutes.
    private void MuteMusic() => SetMusicMuted(true);

    private void UnmuteMusic() => SetMusicMuted(false);

    private void MuteSfx() => SetSfxMuted(true);

    private void UnmuteSfx()
    {
        SetSfxMuted(false);

        // Plays only now that SFX is unmuted, confirming it is back on.
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
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

    private static void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null) button.onClick.AddListener(action);
    }

    private static void RemoveListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null) button.onClick.RemoveListener(action);
    }
}
