using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour
{
    public static GemController instance; // Make sure you set this reference
    public GameObject[] gems; // Array to hold all gem GameObjects
    public GameObject popUp;
    public TMPro.TextMeshProUGUI popUpText;
    public GameObject popUp_2;
    
    private int gemPoints = 0; // Number of gems collected

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

    public void CollectGem()
    {
        gemPoints++;
        if (gemPoints == 3)
        {
            CoinManager.instance.HideCoins(); // Call method in CoinManager to hide all coins
            ShowHint();
        }
        else
        {
            // Update the popup text with remaining gems
            popUp.SetActive(true);
            popUpText.text = "Remaining Gems: " + (3 - gemPoints);
            StartCoroutine(HidePopUpAfterDelay(2f));
        }
    }

    private void ShowHint()
    {
        // Logic to show remaining gems or take other actions
        // For example, activate specific gem GameObjects or provide additional feedback
        popUp_2.SetActive(true);
        StartCoroutine(HidePopUpAfterDelay(2f));
    }

    IEnumerator HidePopUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUp.SetActive(false);
        popUp_2.SetActive(false);
    }
}
