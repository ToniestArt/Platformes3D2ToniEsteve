using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    public static SceneLoader Instance;

    [SerializeField] private GameObject _loadingCanvas;
    [SerializeField] private Image _loadingBar;

    void Awake() //Utilizamos este tipo de IF para evitar duplicados de instancia
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
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    */
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadNewScene(sceneName));
    }

    IEnumerator LoadNewScene(string sceneName)
    {
        //La corrutina vs función, una función se ejecuta entera por huevos, una corrutina puedes pausarla a media ejecución, pausarla x segundos, x frames o directamente cuando suceda algo.
        //Requiere sí o sí de una pausa.

        yield return null;

        _loadingCanvas.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float fakeLoadPercentage = 0;

        while(!asyncLoad.isDone)
        {

            //El último 10% lo carga de golpe, pendiente revisar??? //Versión real, vamos con la "chapuza" para mi juego sencillo:
           // _loadingBar.fillamount = asyncLoad.progress; 

            fakeLoadPercentage += 0.01f;
            Mathf.Clamp01(fakeLoadPercentage);

            _loadingBar.fillAmount = fakeLoadPercentage;
            if(asyncLoad.progress >= 0.9f && fakeLoadPercentage >= 0.99f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
        Time.timeScale = 1;
        _loadingCanvas.SetActive(false);
    }

}