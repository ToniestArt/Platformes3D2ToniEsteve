using UnityEngine;

//Script de estrella recogible. Mismo patrón que Coin.cs:
//· OnTriggerEnter2D detecta al jugador (tag "Player").
//· Llama a GameManager.Instance.AddStar() para incrementar el contador.
//· Reproduce un sonido, apaga sprite y colisionador, y se destruye con retardo.
public class Star : MonoBehaviour
{
    private AudioSource _starAudioSource;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;

    [SerializeField] private AudioClip _starAudio;

    void Awake()
    {
        _starAudioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
    }

    void PlaySFX()
    {
        _starAudioSource.PlayOneShot(_starAudio);
    }

   /* void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddStar();
            PlaySFX();
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }
    */
}
