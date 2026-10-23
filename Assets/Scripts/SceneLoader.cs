using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

//SceneLoader: singleton que carga las escenas de forma ASÍNCRONA con canvas de carga intermedio.
//· Es la ÚNICA puerta de carga del juego: CanvasManager, MenuManager y GameManager
//  piden la carga a "SceneLoader.Instance.ChangeScene(...)" --> nadie llama a "SceneManager" directamente.
//· Sobrevive a los cambios de escena ("DontDestroyOnLoad", patrón profesor) para que "Instance" no se pierda.
//· Patrón de referencia: el SceneLoader del profesor (Víctor/Plataformas2D2026-main).
public class SceneLoader : MonoBehaviour
{
    /*
    - "Instance": referencia ESTÁTICA a la ÚNICA instancia de SceneLoader del juego.
        · "Estática" --> no pertenece a ningún objeto concreto: la comparten todos los scripts.
        · "Pública" --> GameManager ("GameOver()"), CanvasManager ("ChangeScene") y MenuManager la leen desde fuera.
        · Si no hay ningún SceneLoader en la escena, "Instance" es "null" --> la primera llamada saltaría NRE
          (por eso GameManager / CanvasManager la comprueban antes de usarla).
    */
    public static SceneLoader Instance;

    /*
    - "_loadingCanvas": "GameObject" del canvas que se muestra mientras carga la escena (fondo + barra de progreso).
    Con [SerializeField]: privado en el código, pero VISIBLE en el Inspector para arrastrar la referencia.
    --> Lo asigna Toni en el Inspector; si no, "LoadNewScene" saca un LogError legible (no NRE).
    */
    [SerializeField] private GameObject _loadingCanvas;

    /*
    - "_loadingBar": "Image" de la barra de progreso (su relleno se mueve de 0 a 1).
    Con [SerializeField] por el mismo motivo que "_loadingCanvas".
    --> La barra es una "fachada" (ver "LoadNewScene"): la carga real se frena al 90% y hace el último 10% de golpe.
    */
    [SerializeField] private Image _loadingBar;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué estas asignaciones van en Awake y no en otro sitio?
        · "Asignamos referencias" (fijamos el singleton), no intercambiamos información con otros objetos
          --> es el uso "de libro" de Awake (mismo razonamiento que en Heart / PlayerController).
        · Aquí además protegemos el objeto con "DontDestroyOnLoad" (abajo).
    */
    void Awake()
    {
        /*
        - ¿Qué hace este IF? (patrón "Instance != this", igual que en CanvasManager / GameManager)
            · "Instance != null" --> ya existe otra SceneLoader en la escena.
            · "Instance != this" --> y no es ESTE objeto (evita que el original se destruya a sí mismo).
            · Juntos --> "soy una copia": me destruyo y la original sigue siendo "Instance".
            · Si no --> soy la primera: quedo guardada en "Instance".
        */
        if(Instance != null && Instance != this)
        {
            //· "Destroy(gameObject)": destruye ESTE GameObject (la copia duplicada) al terminar el frame.
            Destroy(gameObject);
        }
        else
        {
            //· Guardo la referencia de ESTE objeto en la variable estática para que el resto del juego la use.
            Instance = this;
        }

        /*
        - "DontDestroyOnLoad(gameObject)": hace que Unity NO destruya este objeto al cargar la siguiente escena.
            · Necesario para que el singleton (y su canvas de carga) sobreviva a los cambios de escena:
              sin esto, al cambiar de escena "Instance" volvería a ser nula y "GameOver()" fallaría.
            · Patrón del profesor: su SceneLoader hace exactamente lo mismo, en el mismo sitio.
            · Trampa a tener en cuenta: el objeto debe estar en la raíz de la jerarquía (hijo directo de "Scene"),
              si no, Unity loguea un error y NO lo protege.
        */
        DontDestroyOnLoad(gameObject);
    }

    /*
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    */
    /*
    - ¿Qué hace este método?
        · "PÚBLICA": es la ÚNICA puerta de carga del juego. CanvasManager (botones del Menú / Game Over),
          MenuManager y GameManager ("GameOver()") llaman a "SceneLoader.Instance.ChangeScene(...)".
        · "StartCoroutine(...)": lanza la corrutina "LoadNewScene" SIN esperar a que termine --> el juego no se congela.
    */
    public void ChangeScene(string sceneName)
    {
        //· Lanza la corrutina de carga (ver "LoadNewScene" más abajo).
        StartCoroutine(LoadNewScene(sceneName));
    }

    /*
    - ¿Qué hace esta corrutina?
        · Muestra el canvas de carga, carga la escena "asíncrona" y mueve una barra de progreso "falsa"
          (la carga real se frena al 90%; la barra es una fachada).
        · Al terminar, deja "Time.timeScale = 1" (por si veníamos en pausa) y oculta el canvas.
    - Corrutina vs función:
        · Una función se ejecuta entera de una vez; una corrutina "pausa" su ejecución con "yield return"
          (un frame, unos segundos, o hasta que ocurra algo) y la retoma después.
        · Para "no congelar el juego" mientras carga la escena hace falta esa pausa --> de ahí la corrutina.
        · Contrato: una corrutina SIEMPRE devuelve "IEnumerator" y se lanza con "StartCoroutine".
    */
    IEnumerator LoadNewScene(string sceneName)
    {
        /*
        - "yield return null": pausa la corrutina UN FRAME y la retoma en el frame siguiente.
            · ¿Para qué? Si mostráramos el canvas y arrancáramos la carga en el MISMO frame en que se pide
              "ChangeScene", la escena actual seguiría viva un frame más; al esperar un frame, la UI actual
              termina de pintarse antes de que empiece a cargarse la nueva.
        */
        yield return null;

        /*
        - Guard nulo del canvas de carga:
            · Si el canvas no está asignado (o su objeto ya no existe), "_loadingCanvas" sería nulo
              y "SetActive" lanzaría NullReferenceException.
            · Con el guard, el fallo es un LogError legible en consola en lugar de una excepción.
        */
        if(_loadingCanvas != null)
        {
            //· Muestro el canvas de carga (se vuelve a ocultar al terminar la corrutina, al final).
            _loadingCanvas.SetActive(true);
        }
        else
        {
            //· Sin canvas no hay barra que mostrar, pero la carga PUEDE seguir --> el aviso va a consola y no rompe el juego.
            Debug.LogError("SceneLoader: no hay canvas de carga asignado. Asigna el canvas en el Inspector (_loadingCanvas).");
        }

        /*
        - "SceneManager.LoadSceneAsync(sceneName)": carga la escena ASÍNCRONA y devuelve una "AsyncOperation"
          (un "recibo" con el estado de la carga: "progress", "isDone", "allowSceneActivation").
        */
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        /*
        - Guard de "asyncLoad":
            · "LoadSceneAsync" devuelve NULL (y loguea el error) si la escena no está en EditorBuildSettings.
            · Sin este guard, la línea siguiente saltaría NullReferenceException en silencio.
        */
        if(asyncLoad == null)
        {
            //· La escena no está en el build: no hay nada que cargar --> aviso legible y fin de la corrutina.
            Debug.LogError("SceneLoader: la escena \"" + sceneName + "\" no está en el build. Añádela en Editor > Build Settings.");
            //· "yield break": termina la corrutina AHORA (es el equivalente a un "return" dentro de una corrutina).
            yield break;
        }

        /*
        - "allowSceneActivation = false": la carga avanza hasta el 90% y SE Frena.
            · La ACTIVACIÓN final (el último 10%) no ocurre hasta que la pongamos a "true" nosotros
              --> así controlamos el momento exacto en que cambia la escena (ver el "while" más abajo).
        */
        asyncLoad.allowSceneActivation = false;

        /*
        - "fakeLoadPercentage": progreso de la BARRA FALSA (fachada).
            · La carga real se queda al 90% esperando; la barra avanza a su propio ritmo hasta el 99%.
        */
        float fakeLoadPercentage = 0;

        /*
        - Bucle de carga: mientras la escena no esté terminada ("isDone"):
            · avanzo la barra falsa un 1% por iteración,
            · cuando la carga real llega al 90% Y la barra al 99%, dejo activar la escena,
            · espero 0.1 s de tiempo REAL y repito.
        */
        while(!asyncLoad.isDone)
        {

            //El último 10% lo carga de golpe, pendiente revisar??? //Versión real, vamos con la "chapuza" para mi juego sencillo:
           // _loadingBar.fillamount = asyncLoad.progress;

            /*
            - "fakeLoadPercentage = Mathf.Clamp01(...)": la barra simulada nunca supera 1.0.
                · La versión anterior llamaba a "Mathf.Clamp01(fakeLoadPercentage)" SIN asignar el resultado
                  --> era un no-op (la llamada no hacía nada) y la barra podía pasar de 1.0.
            */
            fakeLoadPercentage = Mathf.Clamp01(fakeLoadPercentage + 0.01f);

            /*
            - Guard nulo de la barra: si no está asignada en el Inspector, no se rompe la carga.
            */
            if(_loadingBar != null)
            {
                //· "Image.fillAmount": el 0-1 de "cuánto" está rellenado el Image (solo tiene efecto con el tipo "Filled").
                _loadingBar.fillAmount = fakeLoadPercentage;
            }
            /*
            - ¿Cuándo dejo activar la escena?
                · "asyncLoad.progress >= 0.9f"  --> la carga real llegó al 90% (donde se frena).
                · "fakeLoadPercentage >= 0.99f" --> la barra falsa casi llega al final (no la cortamos a medias).
                · Con las DOS, la activamos --> la escena cambia y "isDone" pasa a "true" (el bucle termina).
            */
            if(asyncLoad.progress >= 0.9f && fakeLoadPercentage >= 0.99f)
            {
                //· "allowSceneActivation = true": la carga hace el último 10% de golpe y la nueva escena queda activa.
                asyncLoad.allowSceneActivation = true;
            }

            /*
            - "WaitForSecondsRealtime(0.1f)": espera 0.1 s de TIEMPO REAL (sigue corriendo aunque Time.timeScale = 0).
            --> La barra avanza ~10% por segundo (llena en unos 10 s) sin bloquear el juego.
            */
            yield return new WaitForSecondsRealtime(0.1f);
        }
        /*
        - "Time.timeScale = 1": por si la partida estaba EN PAUSA (timeScale = 0) al pedir el cambio de escena:
          con el tiempo congelado, la UI de la nueva escena no respondería hasta despausar.
        */
        Time.timeScale = 1;
        if(_loadingCanvas != null)
        {
            //· Escondo el canvas de carga (se volvió a mostrar al principio de la corrutina).
            _loadingCanvas.SetActive(false);
        }
    }

}
