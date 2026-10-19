using UnityEngine;

public class Heart : MonoBehaviour
{
    //Definimos un campo privado "_heartAudioSource", de tipo "AudioSource". Sin [SerializeField]:
    //lo asignamos nosotros con GetComponent porque es la referencia al componente del MISMO GameObject que lleva este script.
    private AudioSource _heartAudioSource;
    //Definimos un campo privado "_spriteRenderer", de tipo "SpriteRenderer". Sin [SerializeField]:
    //lo asignamos con GetComponent por el mismo motivo que arriba.
    private SpriteRenderer _spriteRenderer;
    //Definimos un campo privado "_collider", de tipo "CircleCollider2D". Sin [SerializeField]:
    //lo asignamos con GetComponent. Es el colisionador que "detecta" al jugador al solapar (trigger).
    private CircleCollider2D _collider;

    //Definimos un campo privado "_healAmaount", de tipo "int". Con [SerializeField].
    //--> Es la cantidad de vida que devuelve el corazón; se configura en el Inspector.
    [SerializeField] private int _healAmaount = 10;
    //Definimos un campo privado "_heartSound", de tipo "AudioClip". Con [SerializeField].
    //--> Es el clip de sonido que se reproduce al recoger el corazón; se arrastra en el Inspector.
    [SerializeField] private AudioClip _heartSound;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué estas asignaciones van en Awake y no en otro sitio?
        · "Asignamos referencias" (GetComponent), no intercambiamos información con otros objetos --> es el uso "de libro" de Awake.
        · Si lo hiciéramos en Start() también funcionaría, pero el patrón del proyecto (PlayerController) lo hace en Awake.
    */
    void Awake()
    {
        /*
        - ¿Qué ocurre realmente cuando solicitamos un "GetComponent<AudioSource>()"?
            · Se busca UN componente de tipo "AudioSource" ADJUNTO AL MISMO GameObject que lleva este script.
            · Se asigna la REFERENCIA obtenida al campo "_heartAudioSource".
            · Si el GameObject no lleva AudioSource, devolverá "Nulo/Null" SILENCIOSAMENTE (sin excepción)
              --> y al llamar después "_heartAudioSource.PlayOneShot(...)" saltaría una "NullReferenceException".
            · El patrón [RequireComponent(typeof(AudioSource))] garantizaría la presencia del componente a nivel de compilación,
              evitando el null silencioso (mismo razonamiento que aplicamos con Animator en PlayerController).
        */
        //Asignamos al campo "_heartAudioSource" la referencia del "AudioSource" de este mismo GameObject.
        _heartAudioSource = GetComponent<AudioSource>();
        //Asignamos al campo "_spriteRenderer" la referencia del "SpriteRenderer" de este mismo GameObject.
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //Asignamos al campo "_collider" la referencia del "CircleCollider2D" de este mismo GameObject.
        _collider = GetComponent<CircleCollider2D>();
    }

    /*
    - ¿Qué hace exactamente este método?
        · Reproduce el sonido del corazón como "one-shot" y no deja nada "colgado" de reproducción.
    */
    void PlaySFX()
    {
        //· "PlayOneShot(clip)": reproduce el clip "una sola vez" ENCIMA del bucle que ya pueda estar sonando.
        //    --> Diferencia con "Play()": Play() arranca la reproducción CONTINUA del clip asignado al AudioSource.
        //    · Coincide con el patrón de "PlaySFX(AudioClip clip)" de PlayerController, pero aquí no usamos volumen
        //      (por defecto es 1, al igual que el parámetro "float volume = 1" de PlayerController).
        _heartAudioSource.PlayOneShot(_heartSound);
    }

    /*
    Unity invoca OnTriggerEnter2D(Collider2D collision) cuando un COLLISIONADOR (de este o del otro objeto)
    toca este colisionador marcado como TRIGGER, y nos pasa por parámetro el colisionador con el que se ha solapado.
    - ¿Por qué "Trigger" y no colisión física?
        · Un "Trigger" NO empuja ni bloquea: solo "avisa" del solape. Perfecto para recoger objetos sin interferir con el movimiento del jugador.
    */
    void OnTriggerEnter2D(Collider2D collision)
    {
        //· "collision.gameObject.CompareTag("Player")": comprueba que el TAG del objeto que me toca es "Player".
        //    --> Es la forma RECOMENDADA de comprobar tags (más legible y a prueba de typos que comparar con "CompareTag" vs "tag ==").
        //    · Sin esta comprobación, el corazón "explotaría" al tocarse con enemigos o paredes que también tienen colisionador.
        if(collision.gameObject.CompareTag("Player"))
        {
            /*
            - ¿Qué ocurre aquí exactamente?
                · "collision.GetComponent<PlayerController>()": se busca el componente "PlayerController" en el GameObject que me toca (el jugador).
                · Mismo contrato de "GetComponent" que en PlayerController: si no lo encuentra devuelve "Nulo/Null" en silencio,
                  y la siguiente línea lanzaría "NullReferenceException".
                  --> Aquí no pasa en la práctica porque si el tag es "Player" SÍ lleva el script (lo usamos en PlayerController para hacerle daño).
            */
            //Asignamos al campo local "_playerScript" la referencia del "PlayerController" del jugador.
            PlayerController _playerScript = collision.GetComponent<PlayerController>();
            //· Llamamos al método PÚBLICO "AddHealth(int)" de PlayerController pasándole la cantidad de vida que devuelve ESTE corazón.
            //    --> PlayerController la suma a su "_actualHealth" y la corta en "_maxHealth" (el corazón no puede subir la vida por encima del máximo).
            //_playerScript.AddHealth(_healAmaount);

            //· Reproduzco el sonido de recoger el corazón (ver "PlaySFX()" más arriba).
            PlaySFX();
            //· "_spriteRenderer.enabled = false": APAGO la renderización del sprite (el corazón deja de verse).
            //    --> Se hace AHORA, en el momento de la recogida, para que no parpadee ni siga apareciendo en el juego los 0.5 s hasta que se destruya.
            _spriteRenderer.enabled = false;
            //· "_collider.enabled = false": DESACTIVO el colisionador (deja de detectar triggers).
            //    --> Evita que, si el jugador sigue solapado, OnTriggerEnter2D se dispare otra vez en el frame siguiente
            //      (recogida doble: dos sonidos, dos curas). Es la misma "precaución anti-doble-disparo" que el colisionador de Goomba.
            _collider.enabled = false;
            //· "Destroy(gameObject, 0.5f)": destruye ESTE GameObject (el corazón) PASADOS 0.5 SEGUNDOS (segundo argumento = retardo).
            //    --> Da tiempo a escuchar el sonido y a que la escena se sienta fluida antes de que el objeto desaparezca del mundo.
            //    · "Destroy(objeto, retardo)" vs "Destroy(objeto)": el retardo es OPCIONAL; sin él la destrucción se produce al terminar el frame.
            Destroy(gameObject, 0.5f);
        }
    }
}
