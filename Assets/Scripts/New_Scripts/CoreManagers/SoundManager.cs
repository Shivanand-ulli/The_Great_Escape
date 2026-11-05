using UnityEngine;

// Each value equals its position in the matching clip array on SoundManager.
// To add a sound: add the clip at the end of the array, then add a new value here with the next number.
public enum Music
{
    MagicEscapeRoom = 0
}

public enum Sfx
{
    Coin = 0,
    ButtonClick = 1,
    LevelWin = 2,
    OpenNewLevel = 3,
    PlayerLose = 4,
    EnemyDeath = 5,
    Sword = 6,
    Pop = 7,
    Jump = 8,
    Collect = 9
}

/// <summary>
/// Plays music and SFX, and handles music/SFX mute. Mute state is saved in PlayerData.
/// </summary>
public class SoundManager : SingletonManager<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    [Header("Settings")]
    [SerializeField] private Music defaultMusic = Music.MagicEscapeRoom;
    [SerializeField] private bool playDefaultMusicOnStart = true;

    private bool musicMuted;
    private bool sfxMuted;

    public bool IsMusicMuted => musicMuted;
    public bool IsSfxMuted => sfxMuted;

    protected override void OnInitialize()
    {
        LoadSavedState();

        if (playDefaultMusicOnStart)
        {
            PlayMusic(defaultMusic);
        }
    }

    /// <summary>Re-reads mute state from PlayerData and applies it.</summary>
    public void LoadSavedState()
    {
        PlayerDataManager dataManager = PlayerDataManager.Instance;
        if (dataManager != null && dataManager.Data != null)
        {
            musicMuted = dataManager.Data.musicMuted;
            sfxMuted = dataManager.Data.sfxMuted;
        }

        ApplyMute();
    }

    // ---------- Music ----------

    /// <summary>Example: SoundManager.Instance.PlayMusic(MusicName.MagicEscapeRoom);</summary>
    public void PlayMusic(Music music, bool loop = true)
    {
        AudioClip clip = GetClip(musicClips, (int)music);
        if (musicSource == null || clip == null) return;

        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;
        if (musicSource != null) musicSource.mute = muted;

        PlayerDataManager dataManager = PlayerDataManager.Instance;
        if (dataManager != null) dataManager.SetMusicMuted(muted);
    }

    public void ToggleMusicMute() => SetMusicMuted(!musicMuted);

    // ---------- SFX ----------

    /// <summary>Example: SoundManager.Instance.PlaySFX(SfxName.Coin);</summary>
    public void PlaySFX(Sfx sfx)
    {
        if (sfxMuted || sfxSource == null) return;

        AudioClip clip = GetClip(sfxClips, (int)sfx);
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void SetSfxMuted(bool muted)
    {
        sfxMuted = muted;
        if (sfxSource != null) sfxSource.mute = muted;

        PlayerDataManager dataManager = PlayerDataManager.Instance;
        if (dataManager != null) dataManager.SetSfxMuted(muted);
    }

    public void ToggleSfxMute() => SetSfxMuted(!sfxMuted);

    // ---------- Helpers ----------

    private void ApplyMute()
    {
        if (musicSource != null) musicSource.mute = musicMuted;
        if (sfxSource != null) sfxSource.mute = sfxMuted;
    }

    private static AudioClip GetClip(AudioClip[] clips, int index)
    {
        if (clips == null || index < 0 || index >= clips.Length)
        {
            Debug.LogWarning($"[SoundManager] No clip assigned at index {index}.");
            return null;
        }
        return clips[index];
    }
}
