using UnityEngine;
using UnityEngine.UI;

public class SettingPannel : MonoBehaviour
{
    public Button musicOnButton;
    public Button musicOffButton;
    public Button sfxOnButton;
    public Button sfxOffButton;

    void Start()
    {
        UpdateMusicButtonState(SoundManager.Instance.IsMusicMuted);
        UpdateSFXButtonState(SoundManager.Instance.IsSfxMuted);
    }

    public void TurnMusicOn()
    {
        SoundManager.Instance.SetMusicMuted(false);
        UpdateMusicButtonState(false);
    }

    public void TurnMusicOff()
    {
        SoundManager.Instance.SetMusicMuted(true);
        UpdateMusicButtonState(true);
    }

    public void TurnSFXOn()
    {
        SoundManager.Instance.SetSfxMuted(false);
        SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
        UpdateSFXButtonState(false);
    }

    public void TurnSFXOff()
    {
        SoundManager.Instance.SetSfxMuted(true);
        UpdateSFXButtonState(true);
    }

    void UpdateMusicButtonState(bool isMuted)
    {
        musicOnButton.gameObject.SetActive(!isMuted);
        musicOffButton.gameObject.SetActive(isMuted);
    }

    void UpdateSFXButtonState(bool isMuted)
    {
        sfxOnButton.gameObject.SetActive(!isMuted);
        sfxOffButton.gameObject.SetActive(isMuted);
    }
}
