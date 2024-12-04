using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private GameObject loosePannel;
    [SerializeField] private TextMeshProUGUI coinsCollected;

    void Start()
    {
        if (GameSessionController.instance != null)
        {
            GameSessionController.instance.loosePannel = loosePannel;
            GameSessionController.instance.coinsCollected = coinsCollected;
        }
        else
        {
            Debug.LogError("GameSessionController instance not found!");
        }
    }
}
