using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Definimos un campo "público" "estático" (static) "Instance", de tipo "AudioManager".
    //--> Es el PATRÓN SINGLETON: con "static" el campo pertenece a la CLASE, no a una instancia concreta.
    //    · Eso significa que "AudioManager.Instance" se puede leer desde CUALQUIER script sin instancia:
    //      "AudioManager.Instance.StartSoundtrack()" y sin importar (using System no hace falta aquí, lo dejamos por el apunte).
    //    · "Instance" actúa como la "punta de anzuelo" única por la que el resto del mundo accede a ESTE objeto.
    public static AudioManager Instance;

    //Definimos un campo privado "_audioSource", de tipo "AudioSource". Sin [SerializeField]:
    //lo asignamos con GetComponent porque es la referencia al componente del MISMO GameObject que lleva este script
    //(el objeto "AudioManager" de la escena, el mismo razonamiento que "_heartAudioSource" de Heart).
    private AudioSource _audioSource;

    //Definimos un campo privado "_level1Soundtrack", de tipo "AudioClip". Con [SerializeField].
    //--> Es la banda sonora del nivel 1; se arrastra en el Inspector.
    //    · "AudioClip" es el ASSET de audio (el archivo .wav/.mp3 de la carpeta Assets), distinto del "AudioSource"
    //      (el "reproductor" colgado del GameObject). Clip = la cinta; Source = la grabadora.
    [SerializeField]private AudioClip _level1Soundtrack;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué el "control de singleton" va en Awake y no en Start?
        · Awake se ejecuta ANTES que Start de TODOS los objetos de la escena. Si dos AudioManager se activan a la vez,
          el que despierte segundo ya encontrará "Instance" ocupado y se destruirá a sí mismo --> queda uno solo.
        · Si lo hiciéramos en Start, el orden de ejecución entre los dos podría no ser el que esperáramos (Unity no garantiza el orden de Start entre scripts).
        · Además, igual que en PlayerController y Heart: aquí solo "establecemos referencias", que es el uso de libro de Awake.
    */
    void Awake()
    {
        /*
        - ¿Qué comprueba exactamente esta condicional?
            · "Instance != null": ya existe UN AudioManager registrado (uno creado antes que yo).
            · "Instance != this": ese registrado NO soy yo (protección contra re-ejecución de Awake sobre el mismo objeto).
            · Juntas: "hay otro AudioManager vivo que no soy yo" --> este objeto es UN DUPLICADO y no debe existir.
            · "Destroy(gameObject)": destruye ESTE GameObject (el duplicado), dejando vivo al original.
              --> A diferencia de "DestroyImmediate", Destroy() lo hace al terminar el frame; es la forma "correcta" durante Awake.
        */
        if(Instance != null && Instance != this)
        {
            //· Este soy yo, el duplicado: me destruyo para que no queden dos "instancias únicas".
            Destroy(gameObject);
        }
        else
        {
            /*
            - "Instance = this": "this" es la REFERENCIA al MISMO objeto sobre el que se está ejecutando este Awake.
              --> Registramo a nosotros mismos como LA instancia del singleton: a partir de aquí,
              cualquier script hace "AudioManager.Instance.PauseSoundtrack()" y le da igual dónde está el objeto en la jerarquía.
            */
            Instance = this;
        }

        /*
        - ¿Qué ocurre realmente cuando solicitamos un "GetComponent<AudioSource>()"?
            · Mismo contrato que en Heart/PlayerController: busca UN componente "AudioSource" ADJUNTO AL MISMO GameObject
              que lleva este script y asigna la REFERENCIA al campo "_audioSource".
            · Si el objeto "AudioManager" no llevase AudioSource, devolvería "Nulo/Null" SILENCIOSAMENTE
              --> y al llamar después "_audioSource.clip = ..." saltaría una "NullReferenceException".
            · El patrón [RequireComponent(typeof(AudioSource))] lo impediría a nivel de compilación (misma idea que con Animator).
        */
        //Asignamos al campo "_audioSource" la referencia del "AudioSource" de este mismo GameObject.
        _audioSource = GetComponent<AudioSource>();
    }

    /*
    - ¿Qué hace exactamente este método?
        · CARGA la banda sonora del nivel 1 y la pone a reproducir en BUCLE (música de fondo).
        · Se llama "público" porque el resto del mundo (GameManager al arrancar la partida, SceneLoader al cambiar de nivel)
          lo invoca a través del singleton: "AudioManager.Instance.StartSoundtrack()".
    */
    public void StartSoundtrack()
    {
        //· "_audioSource.clip = ...": CAMBIAMOS la "cinta" que tiene cargada el reproductor por la del Inspector.
        //    --> Primero cargamos el clip y DESPUÉS Play(); si hicimos Play() sin clip no sonaría nada.
        _audioSource.clip = _level1Soundtrack;
        //· "Play()": arranca la reproducción CONTINUA del clip asignado al AudioSource (música de fondo).
        //    --> Diferencia con "PlayOneShot(clip)" de Heart/PlayerController: ese reproduce un efecto "una sola vez"
        //      ENCIMA del bucle; "Play()" es el bucle principal (la BGM) que luego se puede "Pause()".
        //    · Si el clip tiene "Loop" marcado en el Inspector, se repetirá automáticamente; si no, terminará y callarás.
        _audioSource.Play();
    }

    /*
    - ¿Qué hace exactamente este método?
        · PONE EN PAUSA la reproducción actual SIN borrarla: "Pause()" congela la música donde iba.
        · Llamar de nuevo "Play()" (o "StartSoundtrack()") la reanuda DESDE ESE MISMO PUNTO, no desde el principio.
        · Lo usa GameManager al hacer Pause (mismos "apuntes profe" que tenemos en PlayerController con "GameManager.Instance.Pause()").
        · Diferencia con "Stop()": ese sí cortaría por completo y "Play()" volvería a empezar desde el segundo 0.
    */
    public void PauseSoundtrack()
    {
        //· Congela la BGM en el instante actual; no destruye nada ni recarga el clip.
        _audioSource.Pause();
    }
}
