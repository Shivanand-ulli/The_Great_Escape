using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
     public GameObject popup;
    bool keyCollected = false;
    void Awake()
    {
        popup.SetActive(false);
    }
    
    public bool isKeyCollected()
    {
        return keyCollected;
    }

    public void collectKey()
    {
        keyCollected = true;
    }

    public void textAppear()
    {
        popup.SetActive(true);
    }

    public void textDisappear()
    {
        popup.SetActive(false);
    }

    public void PopUp()
    {
        textAppear();
        StartCoroutine(popUpAppear(5f));
    }

    IEnumerator popUpAppear(float delay)
    {
        yield return new WaitForSeconds(delay);
        textDisappear();
    }

}
