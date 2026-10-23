using UnityEngine;

//MenuManager: acompañante del MENÚ (botones Play / Menu / Quit).
//· No carga nada por su cuenta: delega la carga en SceneLoader (la ÚNICA puerta de carga del juego).
//· Cierra la aplicación (QuitGame).
//· ESTADO ACTUAL (verificado por GUID, 2026-10-23): todavía NO lo usa ninguna escena
//  (Assets/Scenes solo tiene "Nivel 1" + "SampleScene").
//  Se cableará cuando Toni construya el menú (fase UI; patrón de la "Main Menu" del profesor).
public class MenuManager : MonoBehaviour
{
    /*
    - ¿Qué hace este método?
        · "PÚBLICA": es la puerta de entrada de los BOTONES del menú (Play, reintentar, volver al menú).
        · Delega en "SceneLoader.Instance.ChangeScene(...)": el botón no necesita saber cómo carga la escena
          (canvas, asíncrono, barra falsa...); solo pide "carga esta escena" (ver SceneLoader).
        · "sceneName" llega como string desde el Inspector (el nombre exacto de la escena lo fija Toni
          al cablear el botón: p. ej. "Nivel 1").
        ⚠ PENDIENTE (NRE latente de la auditoría): "SceneLoader.Instance" sin guard.
        Si no hubiera ningún SceneLoader en la escena, aquí saltaría NullReferenceException.
        (Queda fuera de los 3 fixes acordados; se añade en el pase de guards.)
    */
    public void ChangeScene(string sceneName)
    {
        //· P. ej. el botón "Play" del menú llamaría a ChangeScene("Nivel 1").
        SceneLoader.Instance.ChangeScene(sceneName);
    }

    /*
    - ¿Qué hace este método?
        · "PÚBLICA": puerta de entrada del botón "Quit" del menú.
        · "Application.Quit()": cierra la aplicación.
        ⚠ TRAMPA a conocer: en el EDITOR de Unity, "Application.Quit()" NO cierra el editor,
        solo sale del MODO PLAY (en la build final sí cierra de verdad).
        --> Si al pulsar "Quit" en el editor "no pasa nada", es este el motivo; no es un bug.
    */
    public void QuitGame()
    {
        Application.Quit();
    }

}
