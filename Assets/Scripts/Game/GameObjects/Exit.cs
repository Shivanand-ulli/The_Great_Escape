using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private ParticleSystem exitParticleSystem;

    private KeyManager keyManager;
    private bool levelEnded;

    private void Start()
    {
        keyManager = KeyManager.Instance;
        if (keyManager == null) return;

        keyManager.OnKeyCollected += ShowPanel;
    }

    private void OnDestroy()
    {
        if (keyManager != null) keyManager.OnKeyCollected -= ShowPanel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelEnded || !other.CompareTag("Player")) return;
        if (keyManager == null || !keyManager.IsKeyCollected) return;

        levelEnded = true;

        int coins = 0;
        CoinManager coinManager = CoinManager.Instance;
        if (coinManager != null)
        {
            coinManager.CommitCoins();
            coins = coinManager.coinPoints;
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.LevelWin);

        // Saves progress and sets the game state to Won.
        if (LevelManager.Instance != null) LevelManager.Instance.LevelCompleted(coins, coins);

        if (PopupManager.Instance != null) PopupManager.Instance.ShowPopup<LevelCompletedPopup>();
    }

    private void ShowPanel()
    {
        if (exitDoor != null) exitDoor.SetActive(true);
        if (exitParticleSystem != null) exitParticleSystem.Play();
    }
}
