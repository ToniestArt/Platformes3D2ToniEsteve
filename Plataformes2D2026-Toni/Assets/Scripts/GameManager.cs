using UnityEngine;

//GameManager: singleton que centraliza el estado de la partida.
//· Contadores: monedas, estrellas (actual / total).
//· Vida del jugador (la gestiona aquí para que Mimik, Heart y PlayerController no se crucen).
//· Condiciones de fin: Victoria (todas las estrellas) / Game Over (vida 0 o caída).
//· Pausa: congela Time.timeScale y la BGM.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //--- Contadores ---
    [SerializeField] private int coins;
    [SerializeField] private int stars;
    [SerializeField] private int totalStars = 3;   //Cuantas estrellas hay en el nivel

    //--- Vida del jugador ---
    [SerializeField] private int playerHealth = 100;
    [SerializeField] private int maxPlayerHealth = 100;

    //--- Estado ---
    private bool _isPaused = false;
    private bool _gameOver = false;
    private bool _victory = false;

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

    void Start()
    {
        AudioManager.Instance.StartSoundtrack();
    }

    //--- Monedas ---
    public void AddCoin()
    {
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
    //--- Vida del jugador ---
    public void PlayerTakeDamage(int damage)
    {
        if(_gameOver || _victory) return;
        playerHealth -= damage;
        if(playerHealth <= 0)
        {
            playerHealth = 0;
            GameOver();
        }
    }

    public void PlayerHeal(int amount)
    {
        if(_gameOver || _victory) return;
        playerHealth += amount;
        if(playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }
    }

    //--- Getters para la UI ---
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

    public void GameOver()
    {
        if(_gameOver || _victory) return;
        _gameOver = true;
        Time.timeScale = 1; //Aseguramos que no esté en pausa para que la carga funcione
        SceneLoader.Instance.ChangeScene("GameOver");
    }

    //--- Pausa ---
    public void Pause()
    {
        if(_isPaused)
        {
            _isPaused = false;
            AudioManager.Instance.StartSoundtrack();
            Time.timeScale = 1;
        }
        else
        {
            _isPaused = true;
            AudioManager.Instance.PauseSoundtrack();
            Time.timeScale = 0;
        }

        //CanvasManager.Instance.ChangeCanvasStatus(CanvasManager.Instance.pauseCanvas, CanvasManager.Instance.resumeButton);
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public bool IsGameOver() => _gameOver;
    public bool IsVictory() => _victory;
}
