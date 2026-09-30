using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]private int coins;

    private bool _isPaused = false;

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

    public void AddCoin()
    {
        coins += 1;
    }

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

        CanvasManager.Instance.ChangeCanvasStatus();
    }

    public bool IsPaused()
    {
        return _isPaused;
    }
}
