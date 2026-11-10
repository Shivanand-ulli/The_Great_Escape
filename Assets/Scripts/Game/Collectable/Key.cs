using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private ParticleSystem keyVfx;

    private BoxCollider2D myBoxCollider;
    private GameObject visual;

    private void Awake()
    {
        myBoxCollider = GetComponent<BoxCollider2D>();
        if (transform.childCount > 0) visual = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Switch the collider off first so the key can only be picked up once.
        myBoxCollider.enabled = false;

        // KeyManager tells the exit door to open.
        if (KeyManager.Instance != null) KeyManager.Instance.CollectKey();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.Collect);
        if (keyVfx != null) keyVfx.Play();
        if (visual != null) visual.SetActive(false);
    }
}
