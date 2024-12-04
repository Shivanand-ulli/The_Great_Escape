using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSessionController : MonoBehaviour
{
    public static GameSessionController instance;
    public int playerLives = 3;
    private int previousSceneIndex;
    public GameObject loosePannel;
    public TextMeshProUGUI coinsCollected;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.buildIndex == 1 && previousSceneIndex == 0) // Assuming index 1 is the game scene and index 0 is the start menu
        {
            UIManager.instance.hintPannel.SetActive(true);
            playerLives = 3; // Reset lives only if coming from the start menu
            CoinManager.instance.ResetScore(); // Reset the score when starting a new game

            // GemController.ResetGemPoints(0);

            StartCoroutine(play());
        }
        else if (scene.buildIndex > 1 && (previousSceneIndex == 0 || previousSceneIndex == 1))
        {
            UIManager.instance.hintPannel.SetActive(true);
            playerLives = 3;
            CoinManager.instance.ResetScore();

            // GemController.ResetGemPoints(0);
            StartCoroutine(play());
        }

        previousSceneIndex = scene.buildIndex;
        IEnumerator play()
        {
            yield return new WaitForSeconds(0.5f);
            // AudioManager.instance.PlaySFX(3);
        }


        // Only initialize if the scene is a level scene (assuming build index > 0 for levels)
        // if (scene.buildIndex > 0)
        // {
        //     InitializeLevelSceneReferences();
        // }
    }

    // public void InitializeLevelSceneReferences()
    // {
    //     Debug.Log("Level scene references initialized");
    // }

    public void ProcessOfPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            playerLives = 0; // Ensure playerLives is zero
            ResetGameSession();
        }
    }

    void ResetGameSession()
    {
        FindObjectOfType<ScenePersist>()?.ResetScenePersist();
        UIManager.instance.livesTxt.text = playerLives.ToString();
        if(loosePannel != null)
        {
            UIManager.instance.ShowLoosePanel();
            coinsCollected.text = CoinManager.instance.coinPoints.ToString();
            AudioManager.instance.PlaySFX(4);
        }

    }

    void TakeLife()
    {
        playerLives--;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        UIManager.instance.livesTxt.text = playerLives.ToString();
    }
}
