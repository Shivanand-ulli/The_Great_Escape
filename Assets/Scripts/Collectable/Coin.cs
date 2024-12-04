using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public CoinManager coinManager;
    bool wasCollected = false;
    public ParticleSystem coinVfx;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !wasCollected)
        {
            wasCollected = true;
            // coinSparkelEffect.Play();
            CoinManager.instance.CollectCoin();
            CoinManager.instance.coinScore();
            AudioManager.instance.PlaySFX(0);
            coinVfx.Play();
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }  
}
