using UnityEngine;

//Script de estrella recogible. Mismo patrón que Coin.cs:
//· OnTriggerEnter2D detecta al jugador (tag "Player").
//· Llama a GameManager.Instance.AddStar() para incrementar el contador.
//· Reproduce un sonido, apaga sprite y colisionador, y se destruye con retardo.
public class Star : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    //Definimos un campo privado "_starAudioSource", de tipo "AudioSource". Sin [SerializeField]:
    //lo asignamos nosotros con GetComponent porque es la referencia al componente del MISMO GameObject que lleva este script.
    private AudioSource _starAudioSource;
    //Definimos un campo privado "_spriteRenderer", de tipo "SpriteRenderer". Sin [SerializeField]:
    //lo asignamos con GetComponent por el mismo motivo que arriba.
    private SpriteRenderer _spriteRenderer;
    //Definimos un campo privado "_collider", de tipo "CircleCollider2D". Sin [SerializeField]:
    //lo asignamos con GetComponent. Es el colisionador que "detecta" al jugador al solapar (trigger).
    private CircleCollider2D _collider;

    //Definimos un campo privado "_starAudio", de tipo "AudioClip". Con [SerializeField].
    //--> Es el clip de sonido que se reproduce al recoger la estrella; se arrastra en el Inspector.
    [SerializeField] private AudioClip _starAudio;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué estas asignaciones van en Awake y no en otro sitio?
        · "Asignamos referencias" (GetComponent), no intercambiamos información con otros objetos --> es el uso "de libro" de Awake.
        · Si lo hiciéramos en Start() también funcionaría, pero el patrón del proyecto (Heart, PlayerController, Coin) lo hace en Awake.
    */
    void Awake()
    {
        /*
        - ¿Qué ocurre realmente cuando solicitamos un "GetComponent<AudioSource>()"?
            · Se busca UN componente de tipo "AudioSource" ADJUNTO AL MISMO GameObject que lleva este script.
            · Se asigna la REFERENCIA obtenida al campo "_starAudioSource".
            · Si el GameObject no lleva AudioSource, devolverá "Nulo/Null" SILENCIOSAMENTE (sin excepción)
              --> y al llamar después "_starAudioSource.PlayOneShot(...)" saltaría una "NullReferenceException".
            · El patrón [RequireComponent(typeof(AudioSource))] garantizaría la presencia del componente a nivel de compilación,
              evitando el null silencioso (mismo razonamiento que en Heart).
        */

         _boxCollider = GetComponent<BoxCollider2D>();
        //Asignamos al campo "_starAudioSource" la referencia del "AudioSource" de este mismo GameObject.
        _starAudioSource = GetComponent<AudioSource>();
        //Asignamos al campo "_spriteRenderer" la referencia del "SpriteRenderer" de este mismo GameObject.
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //Asignamos al campo "_collider" la referencia del "CircleCollider2D" de este mismo GameObject.
        _collider = GetComponent<CircleCollider2D>();
    }

    /*
    - ¿Qué hace exactamente este método?
        · Reproduce el sonido de recoger la estrella como "one-shot" y no deja nada "colgado" de reproducción.
    */
    void PlaySFX()
    {
        //· "PlayOneShot(clip)": reproduce el clip "una sola vez" ENCIMA del bucle que ya pueda estar sonando (la BGM).
        //    --> Diferencia con "Play()": Play() arranca la reproducción CONTINUA del clip asignado al AudioSource.
        //    · Mismo patrón que "PlaySFX()" de Heart y de Coin.
        _starAudioSource.PlayOneShot(_starAudio);
    }

    //⚠ PENDIENTE (fase de victoria, COMENTADO A PROPÓSITO): el método de recogida está DESACTIVADO.
    //· "AddStar()" de GameManager es la puerta de la VICTORIA: al llegar a "totalStars" salta "Victory()"
    //  (y "ShowVictory()" en CanvasManager, por crear). Por eso la estrella no se recoge todavía:
    //  la cadena "recoger -> contar -> victoria" se activa JUNTA en la fase de victoria, no a medias.
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        //· "collision.gameObject.CompareTag("Player")": solo el jugador recoge la estrella (mismo patrón que Coin).
        if(collider2D.gameObject.CompareTag("Player"))
        {
            //· "GameManager.Instance.AddStar()": una estrella más en el contador (al llegar a "totalStars", "Victory()").
            //  ⚠ "GameManager.Instance" sin guard: entra en el pase de guards (mismo apunte que Coin).
            GameManager.Instance.AddStar();
            //· Sonido de recogida (ver "PlaySFX()" más arriba).
            PlaySFX();
            //· Apago el sprite AHORA para que no siga visible durante el retardo de la destrucción.
            _spriteRenderer.enabled = false;
            //· Desactivo el colisionador: anti-recogida-doble (mismo patrón que Heart/Coin).
            _boxCollider.enabled = false;
            //· Destruyo la estrella pasados 0.5 s (da tiempo a escuchar el sonido).
            Destroy(gameObject, 0.5f);
        }
    }
    
}
