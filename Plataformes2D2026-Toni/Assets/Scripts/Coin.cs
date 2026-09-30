using UnityEngine;

public class Coin : MonoBehaviour
{
    private AudioSource _coinAudioSource;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;

    [SerializeField]private AudioClip _coinAudio;

    void Awake()
    {
        _coinAudioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
    }

    void PlaySFX()
    {
        _coinAudioSource.PlayOneShot(_coinAudio);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddCoin();
            PlaySFX();
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }
}