using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    
    public Exit exit;
    KeyManager keyManager;
    public ParticleSystem keyVfx;
    BoxCollider2D myBoxCollider;

    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        exit = FindObjectOfType<Exit>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            keyManager.collectKey();
            keyManager.PopUp();
            print("Key PickedUp");
            keyVfx.Play();
            AudioManager.instance.PlaySFX(9);
            transform.GetChild(0).gameObject.SetActive(false);
            myBoxCollider.enabled = false; 
            exit.showPannel();
        }
    }
}
