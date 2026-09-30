using UnityEngine;

public class Heart : MonoBehaviour
{
    private AudioSource _heartAudioSource;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;

    [SerializeField] private int _healAmaount = 10;
    [SerializeField] private AudioClip _heartSound;

    void PlaySFX()
    {
        _heartAudioSource.PlayOneShot(_heartSound);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController _playerScript = collision.GetComponent<PlayerController>();
            _playerScript.AddHealth(_healAmaount);

            PlaySFX();
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }
}
