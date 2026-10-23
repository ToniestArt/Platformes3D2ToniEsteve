using UnityEngine;

public class Coin : MonoBehaviour
{
    //Definimos un campo privado "_coinAudioSource", de tipo "AudioSource". Sin [SerializeField]:
    //lo asignamos nosotros con GetComponent porque es la referencia al componente del MISMO GameObject que lleva este script.
    private AudioSource _coinAudioSource;
    //Definimos un campo privado "_spriteRenderer", de tipo "SpriteRenderer". Sin [SerializeField]:
    //lo asignamos con GetComponent por el mismo motivo que arriba.
    private SpriteRenderer _spriteRenderer;
    //Definimos un campo privado "_collider", de tipo "CircleCollider2D". Sin [SerializeField]:
    //lo asignamos con GetComponent. Es el colisionador que "detecta" al jugador al solapar (trigger).
    private CircleCollider2D _collider;

    //Definimos un campo privado "_coinAudio", de tipo "AudioClip". Con [SerializeField].
    //--> Es el clip de sonido que se reproduce al recoger la moneda; se arrastra en el Inspector.
    [SerializeField]private AudioClip _coinAudio;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué estas asignaciones van en Awake y no en otro sitio?
        · "Asignamos referencias" (GetComponent), no intercambiamos información con otros objetos --> es el uso "de libro" de Awake.
        · Si lo hiciéramos en Start() también funcionaría, pero el patrón del proyecto (Heart, PlayerController) lo hace en Awake.
    */
    void Awake()
    {
        /*
        - ¿Qué ocurre realmente cuando solicitamos un "GetComponent<AudioSource>()"?
            · Se busca UN componente de tipo "AudioSource" ADJUNTO AL MISMO GameObject que lleva este script.
            · Se asigna la REFERENCIA obtenida al campo "_coinAudioSource".
            · Si el GameObject no lleva AudioSource, devolverá "Nulo/Null" SILENCIOSAMENTE (sin excepción)
              --> y al llamar después "_coinAudioSource.PlayOneShot(...)" saltaría una "NullReferenceException".
            · El patrón [RequireComponent(typeof(AudioSource))] garantizaría la presencia del componente a nivel de compilación,
              evitando el null silencioso (mismo razonamiento que en Heart).
        */
        //Asignamos al campo "_coinAudioSource" la referencia del "AudioSource" de este mismo GameObject.
        _coinAudioSource = GetComponent<AudioSource>();
        //Asignamos al campo "_spriteRenderer" la referencia del "SpriteRenderer" de este mismo GameObject.
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //Asignamos al campo "_collider" la referencia del "CircleCollider2D" de este mismo GameObject.
        _collider = GetComponent<CircleCollider2D>();
    }

    /*
    - ¿Qué hace exactamente este método?
        · Reproduce el sonido de recoger la moneda como "one-shot" y no deja nada "colgado" de reproducción.
    */
    void PlaySFX()
    {
        //· "PlayOneShot(clip)": reproduce el clip "una sola vez" ENCIMA del bucle que ya pueda estar sonando (la BGM).
        //    --> Diferencia con "Play()": Play() arranca la reproducción CONTINUA del clip asignado al AudioSource.
        //    · Mismo patrón que "PlaySFX()" de Heart y "PlaySFX(AudioClip clip)" de PlayerController, pero sin volumen
        //      (por defecto es 1, igual que el parámetro "float volume = 1" de PlayerController).
        _coinAudioSource.PlayOneShot(_coinAudio);
    }

    /*
    Unity invoca OnTriggerEnter2D(Collider2D collision) cuando un COLLISIONADOR (de este o del otro objeto)
    toca este colisionador marcado como TRIGGER, y nos pasa por parámetro el colisionador con el que se ha solapado.
    - ¿Por qué "Trigger" y no colisión física?
        · Un "Trigger" NO empuja ni bloquea: solo "avisa" del solape. Perfecto para recoger monedas sin interferir con el movimiento del jugador.
    */
    void OnTriggerEnter2D(Collider2D collision)
    {
        //· "collision.gameObject.CompareTag("Player")": comprueba que el TAG del objeto que me toca es "Player".
        //    --> Es la forma RECOMENDADA de comprobar tags (más legible y a prueba de typos que comparar con "tag ==").
        //    · Sin esta comprobación, la moneda "reaccionaría" al tocarse con enemigos o paredes que también tienen colisionador.
        if(collision.gameObject.CompareTag("Player"))
        {
            /*
            - ¿Qué ocurre aquí exactamente?
                · "GameManager.Instance.AddCoin()": sube en 1 el contador de monedas de la partida (lo verás en la UI con "GetCoins()").
                · La economía se gestiona CENTRALIZADAMENTE en el singleton GameManager (mismo razonamiento que la vida en Heart,
                  "GameManager.Instance.PlayerHeal(...)"), no en el propio script de la moneda.
                ⚠ PENDIENTE (NRE latente de la auditoría): "GameManager.Instance" sin guard.
                  Si no hubiera ningún GameManager en la escena, aquí saltaría NullReferenceException.
                  (Queda fuera de los 3 fixes acordados; se añade en el pase de guards.)
            */
            //· Llamamos al método PÚBLICO "AddCoin()" de GameManager: una moneda más en el contador de la partida.
            GameManager.Instance.AddCoin();

            //· Reproduzco el sonido de recoger la moneda (ver "PlaySFX()" más arriba).
            PlaySFX();
            //· "_spriteRenderer.enabled = false": APAGO la renderización del sprite (la moneda deja de verse).
            //    --> Se hace AHORA, en el momento de la recogida, para que no siga apareciendo en el juego los 0.5 s hasta que se destruya.
            _spriteRenderer.enabled = false;
            //· "_collider.enabled = false": DESACTIVO el colisionador (deja de detectar triggers).
            //    --> Evita que, si el jugador sigue solapado, OnTriggerEnter2D se dispare otra vez en el frame siguiente
            //      (recogida doble: dos sonidos, dos monedas contadas). Es la misma "precaución anti-doble-disparo" que en Heart.
            _collider.enabled = false;
            //· "Destroy(gameObject, 0.5f)": destruye ESTE GameObject (la moneda) PASADOS 0.5 SEGUNDOS (segundo argumento = retardo).
            //    --> Da tiempo a escuchar el sonido y a que la escena se sienta fluida antes de que el objeto desaparezca del mundo.
            //    · "Destroy(objeto, retardo)" vs "Destroy(objeto)": el retardo es OPCIONAL; sin él la destrucción se produce al terminar el frame.
            Destroy(gameObject, 0.5f);
        }
    }
}
