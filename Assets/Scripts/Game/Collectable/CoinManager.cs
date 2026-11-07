using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
     public static CoinManager instance;
    public GameObject key;
    int totalCoins;
    int sumCoins;
    int collectedCoins = 0;

    [SerializeField] TextMeshProUGUI cointext;
    public GameObject popUp;

    public int coinPoints = 0;
    private GameObject[] allCoins;
    bool isHide = false;

    void Awake()
    {
        if (instance == null)
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
        allCoins = GameObject.FindGameObjectsWithTag("Coin");
        sumCoins = allCoins.Length;
        print("Level total coins is :" + sumCoins);
        key.SetActive(false);
        popUp.SetActive(false);
        cointext.text = coinPoints.ToString();
    }

    public void CollectCoin()
    {
        collectedCoins++;
        if (collectedCoins >= sumCoins)
        {
            print("coinsCollected");
            key.SetActive(true);
            popUpSetActive();
        }
    }

    public void coinScore()
    {
        coinPoints++;
        cointext.text = coinPoints.ToString();
    }

    public void ResetScore()
    {
        coinPoints = 0;
        collectedCoins = 0;
        cointext.text = coinPoints.ToString();
    }

    public void HideCoins()
    {
        if (!isHide)
        {
            HideAllCoins();
            isHide = true;
        }
    }
    public void HideAllCoins()
    {
        allCoins = GameObject.FindGameObjectsWithTag("Coin");

        foreach (GameObject coin in allCoins)
        {
            coin.SetActive(false); // Hide the coins
        }

        // Update coinPoints with the total number of coins hidden
        coinPoints = sumCoins;
        cointext.text = coinPoints.ToString();

        // Save the hidden coins to PlayerPrefs
        PlayerPrefs.SetInt("HiddenCoins", coinPoints);
        PlayerPrefs.Save();

        popUpSetActive();
        key.SetActive(true);
    }


    public void SaveTotalCoins()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        totalCoins += coinPoints;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
    }

    public void LoadTotalCoins(TextMeshProUGUI homeCoinText)
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        homeCoinText.text = totalCoins.ToString();
    }

    void popUpSetActive()
    {
        popUp.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(5f));
    }
    IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUp.SetActive(false);
    }
}
