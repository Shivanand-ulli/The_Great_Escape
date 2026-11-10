using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private ParticleSystem coinVfx;

    private bool wasCollected;
    private GameObject visual;

    private void Start()
    {
        if (transform.childCount > 0) visual = transform.GetChild(0).gameObject;

        if (CoinManager.Instance != null) CoinManager.Instance.RegisterCoin(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wasCollected || !other.CompareTag("Player")) return;
        wasCollected = true;

        if (CoinManager.Instance != null) CoinManager.Instance.CollectCoin();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.Coin);
        if (coinVfx != null) coinVfx.Play();
        if (visual != null) visual.SetActive(false);
    }
}
