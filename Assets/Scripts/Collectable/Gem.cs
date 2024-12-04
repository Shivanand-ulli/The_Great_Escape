using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public ParticleSystem gemParticle;
    public BoxCollider2D boxCollider2D;

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GemController.instance.CollectGem();
            gemParticle.Play();
            AudioManager.instance.PlaySFX(9);
            transform.GetChild(0).gameObject.SetActive(false);
            boxCollider2D.enabled = false;
        }
    }
}
