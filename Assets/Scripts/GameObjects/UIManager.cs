using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{   
    public static UIManager instance;
    public GameObject hintPannel;

    public TextMeshProUGUI livesTxt;

    public GameObject pausePanel,winPanel,conformationPanel,loosePanel;
    public enum panel{none,win,pause,loose};
    public panel currentPanel = panel.none;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        livesTxt.text = GameSessionController.instance.playerLives.ToString();
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        conformationPanel.SetActive(false);
        loosePanel.SetActive(false);
    }

    //Game screen
    public void onPause()
    {
        Time.timeScale = 0;
        playButtonSound();
        ShowPausePanel();
    }

    void ShowPausePanel()
    {
        pausePanel.SetActive(true);
        currentPanel = panel.pause;
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        currentPanel = panel.win;
    }

    public void ShowLoosePanel()
    {
        loosePanel.SetActive(true);
        currentPanel = panel.loose;
    }

    public void ShowConformationPanel()
    {
        conformationPanel.SetActive(true);
    }

    //Pause pannel
    public void onContinue()
    {
        Time.timeScale = 1;
        playButtonSound();
        pausePanel.SetActive(false);
    }
    public void onRestart()
    {
        playButtonSound();
        pausePanel.SetActive(false);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1;
        GameSessionController.instance.playerLives = 3;
    }
    public void onExit()
    {   
        playButtonSound();
        pausePanel.SetActive(false);
        ShowConformationPanel();
    }


    //Warning Pannel

    public void yesButtonPress()
    {
        playButtonSound();
        conformationPanel.SetActive(false);
        SceneManager.LoadScene("HomeScreen");
    }

    public void noButton()
    {
        playButtonSound();
        conformationPanel.SetActive(false);
        if(currentPanel == panel.pause)
        {
            pausePanel.SetActive(true);
        }
        else if(currentPanel == panel.loose)
        {
            loosePanel.SetActive(true);
        }
        else if(currentPanel == panel.win)
        {
            winPanel.SetActive(true);
        }
    }
    public void nextLevel()
    {
        playButtonSound();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void okButton()
    {
        playButtonSound();
        Time.timeScale = 1;
        hintPannel.SetActive(false);
    }



    public void playButtonSound()
    {
        AudioManager.instance.PlaySFX(1);
    }


    public void Retry()
    {
        playButtonSound();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1; 
        GameSessionController.instance.playerLives = 3;
    }


    public void HintButton()
    {
        playButtonSound();
        Time.timeScale = 0;
        hintPannel.SetActive(true);
    }
}

