using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject pauseCanvas;
    public Button resumeButton;

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

    public void ChangeCanvasStatus(GameObject canvas, Button selectedButton)
    {
        if(canvas.activeInHierarchy)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
            //_resumeButton.Select(); //Pendiente de añadir
        }
    }
public void  ChangeScene (string sceneName)
    {
        SceneLoader.Instance.ChangeScene(sceneName);
    }
}