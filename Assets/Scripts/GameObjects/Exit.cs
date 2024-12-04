using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Exit : MonoBehaviour
{
    KeyManager keyManager;

    public TextMeshProUGUI levelCoinText;
    public TextMeshProUGUI homeCoinText;

    public GameObject winPannel;
    public GameObject exitDoor;
    public ParticleSystem exitParticleSystem;

    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && keyManager.isKeyCollected())
        {
            UnlockNewLevel();
            ShowWinPanel();
        }
    }

    public void ShowWinPanel()
    {
        levelCoinText.text = CoinManager.instance.coinPoints.ToString();

        // Save the current coin score to PlayerPrefs
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        totalCoins += CoinManager.instance.coinPoints;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
        AudioManager.instance.PlaySFX(2);
        // keyManager.textDisappear();
        UIManager.instance.ShowWinPanel();
    }


    void UnlockNewLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int reachedIndex = PlayerPrefs.GetInt("ReachedIndex", 0); // Default to 0 if not set

        // Update ReachedIndex only if currentSceneIndex is greater
        if (currentSceneIndex > reachedIndex)
        {
            PlayerPrefs.SetInt("ReachedIndex", currentSceneIndex);
        }

        // Unlock the next level
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Default to 1
        if (unlockedLevel <= currentSceneIndex)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentSceneIndex + 1);
        }

        PlayerPrefs.Save();

        // // Update the button states to reflect the unlocked levels
        // FindObjectOfType<LevelButtonController>().UpdateButtonStates();
    }

    public void showPannel()
    {
        exitDoor.SetActive(true);
        exitParticleSystem.gameObject.SetActive(true);
    }
}
