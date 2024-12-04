using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip[] musicClips;
    public AudioClip[] sfxClips;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PlayMusic(0);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int index, bool loop = true)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = musicClips[index];
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(sfxClips[index]);
    }

    public void ToggleMusic(bool isMuted)
    {
        musicSource.mute = isMuted;
    }

    public void ToggleSFX(bool isMuted)
    {
        sfxSource.mute = isMuted;
    }

    public static void SetMusicState(bool isMuted)
    {
        PlayerPrefs.SetInt("MusicMuted",isMuted ? 1 : 0);
    }

    public static bool GetMusicState()
    {
        return PlayerPrefs.GetInt("MusicMuted", 0) == 1;
    }

    public static void SetSFXState(bool isMuted)
    {
        PlayerPrefs.SetInt("SFXMuted",isMuted ? 1 : 0 );
    }

    public static bool GetSFXState()
    {
        return PlayerPrefs.GetInt("SFXMuted", 0) == 1;
    }

}
