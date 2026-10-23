using UnityEngine;

//GameManager: singleton que centraliza el estado de la partida.
//· Contadores: monedas, estrellas (actual / total).
//· Vida del jugador (la gestiona aquí para que Mimik, Heart y PlayerController no se crucen).
//· Condiciones de fin: Victoria (todas las estrellas) / Game Over (vida 0 o caída).
//· Pausa: congela Time.timeScale y la BGM.
//· Es la ÚNICA "fuente de verdad" de la partida: la UI y los scripts (Star, Coin, Heart, Mimik, PlayerController)
//  la leen / le piden acciones a "GameManager.Instance" sin saber dónde está el objeto.
public class GameManager : MonoBehaviour
{
    /*
    - "Instance": referencia ESTÁTICA a la ÚNICA instancia de GameManager.
        · Igual que "SceneLoader.Instance": el resto del juego la usa sin depender de la posición del objeto.
        · Si no hay ningún GameManager en la escena, "Instance" es "null" --> cualquier llamada saltaría NRE
          (el pase de guards pendiente añade las comprobaciones donde toque).
    */
    public static GameManager Instance;

    //--- Contadores ---
    /*
    - "coins": monedas recogidas en la partida.
    Con [SerializeField]: visible en el Inspector (p. ej. para depurar empezando con monedas).
    --> Se incrementa en "AddCoin()" (lo llama "Coin" al recogerse).
    */
    [SerializeField] private int coins;

    /*
    - "stars": estrellas recogidas hasta ahora.
    --> Se incrementará en "AddStar()" (pendiente, fase de victoria: "Star" la llamará al tocarla).
    */
    [SerializeField] private int stars;

    /*
    - "totalStars": cuántas estrellas hay en el nivel; al llegar a ese número hay VICTORIA.
    --> Decisión pendiente de Toni: ¿3 es definitivo?
    */
    [SerializeField] private int totalStars = 3;   //Cuantas estrellas hay en el nivel

    //--- Vida del jugador ---
    /*
    - "playerHealth": vida actual del jugador (empieza a 100).
    --> "Mimik" la resta ("PlayerTakeDamage") y "Heart" la devuelve ("PlayerHeal").
    */
    [SerializeField] private int playerHealth = 100;

    /*
    - "maxPlayerHealth": tope de vida; "PlayerHeal" no sube por encima.
    */
    [SerializeField] private int maxPlayerHealth = 100;

    //--- Estado ---
    /*
    - "_isPaused": la partida está en pausa (Time.timeScale = 0 + BGM parada).
    Sin [SerializeField]: es estado interno, no se configura desde el Inspector.
    --> Lo lee "IsPaused()" (lo usará el input / la UI de pausa).
    */
    private bool _isPaused = false;

    /*
    - "_gameOver": la partida terminó en derrota (vida 0 o caída).
    --> Bloquea más daños/curas y evita lanzar el cambio de escena dos veces.
    */
    private bool _gameOver = false;

    /*
    - "_victory": la partida terminó en victoria (todas las estrellas).
    --> Igual que "_gameOver": bloquea más cambios de estado (los dos "guardan" los métodos de abajo).
    */
    private bool _victory = false;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué aquí?
        · "Asignamos referencias" (fijamos el singleton), no intercambiamos información con otros objetos
          --> es el uso "de libro" de Awake (mismo patrón que Heart / SceneLoader / CanvasManager).
    */
    void Awake()
    {
        /*
        - ¿Qué hace este IF? (patrón "Instance != this", igual que en SceneLoader / CanvasManager)
            · "Instance != null" --> ya existe otro GameManager en la escena.
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
    }

    /*
    Unity invoca Start() UNA SOLA VEZ, después de que todos los Awake() de la escena hayan terminado.
    - ¿Por qué aquí y no en Awake?
        · Aquí SÍ es seguro preguntar a otros singletons ("AudioManager.Instance"): sus Awake() ya han corrido
          y, si existen, ya han fijado su "Instance".
        · En Awake, el orden entre GameObjects no está garantizado --> "Instance" podría ser aún "null".
    */
    void Start()
    {
        /*
        - Arranco la banda sonora de fondo de la partida.
        Si no hubiera ningún AudioManager en la escena, aquí saltaría NullReferenceException.
        */
        AudioManager.Instance.StartSoundtrack();
    }

    //--- Monedas ---
    /*
    - ¿Qué hace este método?
        · Suma una moneda. Lo llama "Coin" cuando el jugador lo recoge (OnTriggerEnter2D).
    */
    public void AddCoin()
    {
        //· "coins += 1": una moneda más (la UI la mostrará con "GetCoins()").
        coins += 1;
    }

    //--- Estrellas ---
    /*
    public void AddStar()
    {
        stars += 1;
        if(stars >= totalStars)
        {
            Victory();
        }
    }
*/
    /*
    - PENDIENTE (fase de victoria): "AddStar()" sigue COMENTADO a propósito.
        · Al descomentar, "Star" (OnTriggerEnter2D) llamará a "GameManager.Instance.AddStar()" y,
          al llegar a "totalStars", saltará "Victory()".
        · NO descomentar hasta que exista "Victory()" + "CanvasManager.ShowVictory()" (ver más abajo).
    */

    //--- Vida del jugador ---
    /*
    - ¿Qué hace este método?
        · Reste "damage" a la vida del jugador. Lo llama "PlayerController" / "Mimik" al recibir un golpe.
        · Si la vida llega a 0 (o queda negativa) --> "GameOver()".
    */
    public void PlayerTakeDamage(int damage)
    {
        //· Si la partida ya terminó (victoria o derrota), un golpe más no cambia nada --> sales sin hacer nada.
        if(_gameOver || _victory) return;
        //· "playerHealth -= damage": resta el daño (puede quedar negativo; se normaliza abajo).
        playerHealth -= damage;
        /*
        - ¿Y si la vida pasó a 0 o quedó negativa?
            · "playerHealth = 0": la vida NUNCA se muestra negativa en la UI.
            · "GameOver()": deriva la partida (carga la escena "GameOver").
        */
        if(playerHealth <= 0)
        {
            playerHealth = 0;
            GameOver();
        }
    }

    /*
    - ¿Qué hace este método?
        · Sume "amount" a la vida (con tope de "maxPlayerHealth"). Lo llama "Heart" al recogerse.
    */
    public void PlayerHeal(int amount)
    {
        //· Si la partida ya terminó, una cura más no cambia nada --> sales sin hacer nada.
        if(_gameOver || _victory) return;
        //· "playerHealth += amount": suma la cura.
        playerHealth += amount;
        /*
        - ¿Y si pasamos del tope?
            · "playerHealth = maxPlayerHealth": la vida no sube por encima del máximo
              (el excedente de la cura se "desperdicia").
        */
        if(playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }
    }

    //--- Getters para la UI ---
    /*
    - ¿Por qué "getters" y no campos públicos?
        · La UI solo debe LEER el estado; si los campos fueran públicos,
          cualquiera podría escribir "GameManager.Instance.coins = 9999" y romper la partida.
        · Los getters son "expresiones de método" ("=> valor"): un atajo a un método que devuelve el valor.
    */
    public int GetCoins() => coins;
    public int GetStars() => stars;
    public int GetTotalStars() => totalStars;
    public int GetPlayerHealth() => playerHealth;
    public int GetMaxPlayerHealth() => maxPlayerHealth;

    //--- Fin de partida ---
   /* public void Victory()
    {
        if(_victory || _gameOver) return;
        _victory = true;
        Time.timeScale = 0;
        CanvasManager.Instance.ShowVictory();
    }*/
    /*
    - PENDIENTE (fase de victoria): "Victory()" sigue COMENTADO a propósito.
        · Al descomentar: marca "_victory", congela el tiempo y pide a CanvasManager el canvas de victoria.
        · Depende de "CanvasManager.ShowVictory()" (todavía no existe) --> no descomentar sin eso.
    */

    /*
    - ¿Qué hace este método?
        · Marca la derrota y carga la ESCENA "GameOver" (patrón Zombie: el Game Over es una ESCENA, no un canvas).
        · Lo llama "PlayerTakeDamage" al quedarse a 0 de vida (y lo llamará la caída, fase 002.2, pendiente).
    */
    public void GameOver()
    {
        //· Si ya había victoria/derrota, no la sobreescribo --> sales sin hacer nada (evita cargar la escena dos veces).
        if(_gameOver || _victory) return;
        //· Marco la derrota ANTES de cargar la escena (así un segundo golpe no relanza "GameOver()").
        _gameOver = true;
        Time.timeScale = 1; //Aseguramos que no esté en pausa para que la carga funcione
        /*
        - "SceneLoader.Instance.ChangeScene("GameOver")": pido la carga a la ÚNICA puerta de carga del juego (ver SceneLoader).
        ⚠ PENDIENTE (NRE latente de la auditoría): "SceneLoader.Instance" sin guard.
        Si no hubiera ningún SceneLoader en la escena, aquí saltaría NullReferenceException.
        (Queda fuera de los 3 fixes acordados; se añade en el pase de guards.)
        */
        SceneLoader.Instance.ChangeScene("GameOver");
    }

    //--- Pausa ---
    /*
    - ¿Qué hace este método?
        · Alterna el estado de pausa: congela Time.timeScale y la BGM; al reanudar, los restaura.
        · Cablea el canvas de pausa con CanvasManager (mismo patrón que el profesor).
        · Lo llamará el botón PAUSA de la UI (fase de canvas de pausa; el panel lo hace Toni).
    */
    public void Pause()
    {
        /*
        - ¿En qué rama estoy?
            · "_isPaused == true"  --> la partida ESTÁ en pausa: REANUDO.
            · "_isPaused == false" --> la partida NO está en pausa: PAUSO.
        */
        if(_isPaused)
        {
            //· Marco que ya no estamos en pausa.
            _isPaused = false;
            //· Repongo la BGM (la dejamos "a medias" al pausar).
            AudioManager.Instance.StartSoundtrack();
            //· "Time.timeScale = 1": el tiempo vuelve a correr a velocidad normal.
            Time.timeScale = 1;
        }
        else
        {
            //· Marco que ahora estamos en pausa.
            _isPaused = true;
            //· Detengo la BGM (quedará lista para reanudarse donde iba).
            AudioManager.Instance.PauseSoundtrack();
            //· "Time.timeScale = 0": congela el tiempo del juego (físicas, animaciones, "WaitForSeconds" normales).
            Time.timeScale = 0;
        }

        /*
        - ¿Qué hace este bloque?
            · Cablea la pausa con CanvasManager: alterna el canvas de pausa y selecciona el botón de reanudar.
            · Mismo patrón que el profesor: GameManager llama a CanvasManager.Instance.ChangeCanvasStatus
              con los campos PÚBLICOS pauseCanvas / resumeButton (los asigna Toni en el Inspector).
            · Guard de "Instance": si el GameObject de CanvasManager no existe en la escena, "Instance" es nula
              --> en lugar de NullReferenceException, salta un LogError legible en consola.
        */
        if(CanvasManager.Instance != null)
        {
            //· Alterno el canvas de pausa y selecciono el botón indicado (ver CanvasManager.ChangeCanvasStatus).
            CanvasManager.Instance.ChangeCanvasStatus(CanvasManager.Instance.pauseCanvas, CanvasManager.Instance.resumeButton);
        }
        else
        {
            //· Sin CanvasManager no hay canvas que alternar --> aviso legible en consola.
            Debug.LogError("GameManager.Pause: CanvasManager no encontrado en la escena. Asegúrate de que existe el GameObject con este script.");
        }
    }

    /*
    - ¿Qué hace este método?
        · Devuelve si la partida está en pausa. Lo leerá el input / la UI para no procesar movimiento
          mientras está en pausa (p. ej. PlayerController / el botón PAUSA).
    */
    public bool IsPaused()
    {
        return _isPaused;
    }

    /*
    - Getters de fin de partida (misma idea que "GetCoins"): la UI solo lee el estado.
    */
    public bool IsGameOver() => _gameOver;
    public bool IsVictory() => _victory;
}
