using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelButtonController : MonoBehaviour
{
    public Button[] button;
    public Sprite activesprite;
    public Sprite inactivesprite;
    public Sprite completedsprite;


    public Material activeMaterial;
    public Material inactiveMaterial;

    public Vector2 activeSize;
    public Vector2 inactiveSize;


    void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < button.Length; i++)
        {
            //default state is inactive
            button[i].interactable = false;
            button[i].GetComponent<Image>().sprite = inactivesprite;

            SetButtonTextMaterial(button[i], inactiveMaterial);
            SetButtonSize(button[i], inactiveSize);



            //If the level is completed set it to active or completed
            if (i < unlockedLevel)
            {
                button[i].interactable = true;

                if (i < unlockedLevel - 1)
                {
                    //If the level is already completed
                    button[i].GetComponent<Image>().sprite = completedsprite;
                    SetButtonTextMaterial(button[i], activeMaterial);
                    SetButtonSize(button[i], inactiveSize);
                }
                else
                {
                    //If the level is current active level
                    button[i].GetComponent<Image>().sprite = activesprite;
                    SetButtonTextMaterial(button[i], activeMaterial);
                    SetButtonSize(button[i], activeSize);
                }
            }
        }
    }


    // Helper method to set the text material of a button
    void SetButtonTextMaterial(Button button, Material material)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.fontSharedMaterial = material;
        }

    }

    // Helper method to set the size of a button
    void SetButtonSize(Button button, Vector2 size)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = size;
        }
    }

    public void UpdateButtonStates()
    {
        Awake();
    }
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        SceneManager.LoadScene(levelName);
    }


    public void ResetGame()
    {
        // Reset unlocked levels to the first level
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.SetInt("UnlockedLevel", 1);  // Start from the first level again
        PlayerPrefs.Save();

        // Reset button states to reflect the new game state
        UpdateButtonStates();
    }

    public void ResetTotalCoins(TextMeshProUGUI homeCoinText)
    {
        // Reset total coins to 0
        PlayerPrefs.SetInt("TotalCoins", 0);
        PlayerPrefs.Save();

        // Update the home screen UI
        homeCoinText.text = "0";

        // // Reset the in-game coin count if needed
        // ResetScore();
    }
}
