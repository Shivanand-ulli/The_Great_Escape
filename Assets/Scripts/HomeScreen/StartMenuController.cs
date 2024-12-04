using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public TextMeshProUGUI coinsTxt;

    void Start()
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinsTxt.text = totalCoins.ToString();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void sfxButtonSound()
    {
        AudioManager.instance.PlaySFX(1);
    }

    public void nextscene(int i)
    {
        SceneManager.LoadScene(i);
        Time.timeScale = 1.0f;
    }

}
