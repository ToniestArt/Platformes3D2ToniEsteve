using UnityEngine;
using UnityEngine.UI;

//CanvasManager: singleton que gestiona los canvas de UI (pausa / victoria / derrota).
//· Alterna la visibilidad de los canvas y selecciona el botón activo (ChangeCanvasStatus).
//· Enruta las cargas de escena a SceneLoader (no carga escenas directamente).
//· pauseCanvas / resumeButton son PÚBLICOS: los cablea GameManager (mismo patrón que el profesor).
public class CanvasManager : MonoBehaviour
{
    /*
    - "Instance": referencia ESTÁTICA a la ÚNICA instancia de CanvasManager.
        · Igual que en GameManager / SceneLoader: el resto del juego pide la acción a "CanvasManager.Instance"
          sin saber dónde está el objeto.
        · Si no hay ningún CanvasManager en la escena, "Instance" es "null" --> GameManager comprueba antes
          de llamar (guard con LogError).
    */
    public static CanvasManager Instance;

    /*
    - ¿Por qué son PÚBLICOS y no [SerializeField] privados?
        · "GameManager" los cablea desde fuera (CanvasManager.Instance.pauseCanvas / .resumeButton):
          necesitan ser accesibles. Mismo patrón que el profesor (pauseCanvas / resumeButton públicos).
        · Igual que con [SerializeField], siguen apareciendo en el Inspector para asignar las referencias.
    - ¿Qué son?
        · "pauseCanvas":  "GameObject" del panel de PAUSA (se muestra / oculta con "ChangeCanvasStatus").
        · "resumeButton": "Button" que se focaliza al abrir el panel (recibe Enter / Space sin ratón).
        --> Los asigna Toni en el Inspector; si no, el guard "canvas != null" evita el NRE.
    */
    public GameObject pauseCanvas;
    public Button resumeButton;

    /*
    - ¿Por qué usamos "Instance != this"?
        · "Awake()" se llama al instanciar el objeto. Si ya existe un CanvasManager en la escena,
          "Instance" no será nulo. Queremos destruir la DUPLICADA y mantener la original.
        · La comprobación anterior ("Instance != null") era errónea porque siempre es true si hay una instancia.
        · Mismo patrón "de libro" de Awake que en Heart: "asignamos referencias", no intercambiamos
          información con otros objetos.
    */
    void Awake()
    {
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
    }

    /*
    - ¿Qué hace este método?
        · Alterna la visibilidad del canvas (pausa/victoria/derrota).
        · Si está activo, lo desactiva. Si está inactivo, lo activa y selecciona el botón indicado.
    */
    public void ChangeCanvasStatus(GameObject canvas, Button selectedButton)
    {
        //· Comprobamos que el canvas no sea nulo antes de tocarlo (si no, "SetActive" saltaría NRE).
        if(canvas != null)
        {
            /*
            - "canvas.activeInHierarchy": true si el canvas (o alguno de sus ANCESTROS) está activo.
            --> Es el estado REAL: cubre el caso del canvas hijo de un panel apagado
              ("activeSelf" solo miraría al propio canvas).
            */
            if(canvas.activeInHierarchy)
            {
                //· Estaba visible --> lo ocultamos (así se "alterna" al llamar de nuevo).
                canvas.SetActive(false);
            }
            else
            {
                //· Estaba oculto --> lo mostramos.
                canvas.SetActive(true);

                /*
                - "selectedButton.Select()" es un método de Unity UI que:
                    1. Focaliza el botón para que reciba eventos del teclado (Enter/Space) automáticamente.
                    2. Cambia visualmente el estado del botón a "Selected" (si tiene la animación configurada).
                */
                //· Si hay botón, lo focalizo; si no (null), no pasa nada (la UI sigue funcionando con ratón).
                if(selectedButton != null) selectedButton.Select();
            }
        }
    }

    /*
    - ¿Por qué usamos "SceneLoader.Instance"?
        · Porque SceneLoader es también un singleton. Llamamos a su método público para pedirle que cargue la escena.
          Esto delega la responsabilidad de la carga en el gestor de escenas centralizado.
        · Guard de "Instance": si no hay SceneLoader en la escena, "Instance" es nula --> LogError en lugar de NRE.
    */
    public void ChangeScene(string sceneName)
    {
        if(SceneLoader.Instance != null)
        {
            //· Delego la carga a la ÚNICA puerta de carga del juego (ver SceneLoader).
            SceneLoader.Instance.ChangeScene(sceneName);
        }
        else
        {
            //· Sin SceneLoader no se puede cargar nada --> aviso legible en consola.
            Debug.LogError("SceneLoader no encontrado en la escena. Asegúrate de que existe el GameObject con este script.");
        }
    }
}
