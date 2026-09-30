using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    [SerializeField] private GameObject _pauseCanvas;

    void Awake()
    {
        if(Instance != null && Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ChangeCanvasStatus()
    {
        if(_pauseCanvas.activeInHierarchy)
        {
            _pauseCanvas.SetActive(false);
        }
        else
        {
            _pauseCanvas.SetActive(true);
        }
    }
}