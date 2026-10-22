using UnityEngine;
using UnityEngine.UI;

//GameManager: singleton que centraliza el estado de la partida.
//· Contadores: monedas, estrellas (actual / total).
//· Vida del jugador (la gestiona aquí para que Mimik, Heart y PlayerController no se crucen).
//· Condiciones de fin: Victoria (todas las estrellas) / Game Over (vida 0 o caída).
//· Pausa: congela Time.timeScale y la BGM.
public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button resumeButton;

    /*
    - ¿Por qué usamos "Instance != this"?
        · "Awake()" se llama al instanciar el objeto. Si ya existe un CanvasManager en la escena, 
          "Instance" no será nulo. Queremos destruir la DUPLICADA y mantener la original.
        · La comprobación anterior ("Instance != null") era errónea porque siempre es true si hay una instancia.
    */
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
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
        if(canvas != null) //Comprobamos que el canvas no sea nulo antes de tocarlo
        {
            if(canvas.activeInHierarchy)
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
                
                /*
                - "selectedButton.Select()" es un método de Unity UI que:
                    1. Focaliza el botón para que reciba eventos del teclado (Enter/Space) automáticamente.
                    2. Cambia visualmente el estado del botón a "Selected" (si tiene la animación configurada).
                */
                if(selectedButton != null) selectedButton.Select(); 
            }
        }
    }

    /*
    - ¿Por qué usamos "SceneLoader.Instance"?
        · Porque SceneLoader es también un singleton. Llamamos a su método público para pedirle que cargue la escena.
          Esto delega la responsabilidad de la carga en el gestor de escenas centralizado.
    */
    public void ChangeScene(string sceneName)
    {
        if(SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ChangeScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneLoader no encontrado en la escena. Asegúrate de que existe el GameObject con este script.");
        }
    }
}
