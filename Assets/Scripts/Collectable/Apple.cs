using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    CoinManager coinManager;
    public ParticleSystem appleVfx;
    public CircleCollider2D circleCollider2D;
    void Start()
    {
        // Find the CoinManager in the scene
        coinManager = FindObjectOfType<CoinManager>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (coinManager != null)
            {
                coinManager.HideCoins();
                appleVfx.Play();
                AudioManager.instance.PlaySFX(9); 
                transform.GetChild(0).gameObject.SetActive(false); 
                circleCollider2D.enabled = false;
            }
            
        }
    }
}
