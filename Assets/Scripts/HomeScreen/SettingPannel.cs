using System.Collections;
using System.Collections.Generic;
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
        UpdateMusicButtonState(AudioManager.GetMusicState());
        UpdateSFXButtonState(AudioManager.GetSFXState());
    }

    public void TurnMusicOn()
    {
        AudioManager.SetMusicState(false);
        UpdateMusicButtonState(false);
        AudioManager.instance.ToggleMusic(false);
    }
    public void TurnMusicOff()
    {
        AudioManager.SetMusicState(true);
        UpdateMusicButtonState(true);
        AudioManager.instance.ToggleMusic(true);
    }
    
    public void TurnSFXOn()
    {
        AudioManager.instance.PlaySFX(1);
        AudioManager.SetSFXState(false);
        UpdateSFXButtonState(false);
        AudioManager.instance.ToggleSFX(false);
    }

    public void TurnSFXOff()
    {
        AudioManager.SetSFXState(true);
        UpdateSFXButtonState(true);
        AudioManager.instance.ToggleSFX(true);
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
