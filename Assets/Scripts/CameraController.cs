using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
    "[Header("Target & Offset")]" agrupa en el Inspector (dibuja un TÍTULO visual en la ventana)
    la SECCIÓN 1 de campos: a quién sigue la cámara y con qué desplazamiento.
    */
    [Header("Target & Offset")]
    //Definimos un campo privado "cameraTarget", de tipo "Transform". Sin [SerializeField]:
    //no se asigna desde el Inspector, se RESUELVE en "Awake()" buscando al jugador por Tag (abajo).
    //Es la REFERENCIA a la que la cámara se pegará cada frame.
    private Transform cameraTarget;
    //Definimos un campo PÚBLICO "cameraOffset", de tipo "Vector3". Editable desde el Inspector.
    //--> Es el DESPLAZAMIENTO de la cámara respecto al jugador: por defecto (0, 1.5f, -10)
    //    (un poco por encima, y en Z -10 para quedar "detrás" del plano de acción 2D).
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -10); 

    /*
    "[Header("Clamping Bounds")]" agrupa en el Inspector la SECCIÓN 2 de campos: los LÍMITES del área
    por la que la cámara puede moverse (para que no salga del nivel).
    */
    [Header("Clamping Bounds")]
    //Definimos un campo PÚBLICO "minCameraPosition", de tipo "Vector2". Editable desde el Inspector.
    //--> Es el rincón INFERIOR-IZQUIERDO de la ventana de visión permitida (los valores MÍNIMOS de X e Y).
    //    ⚠ SETUP: por defecto es (0,0); si no se asigna en el Inspector, "Clamp" clavará la cámara al origen.
    public Vector2 minCameraPosition;
    //Definimos un campo PÚBLICO "maxCameraPosition", de tipo "Vector2". Editable desde el Inspector.
    //--> Es el rincón SUPERIOR-DERECHO de la ventana de visión permitida (los valores MÁXIMOS de X e Y).
    //    ⚠ SETUP: por defecto es (0,0); si min == max, la cámara queda "clavada" en ese punto.
    public Vector2 maxCameraPosition;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué la búsqueda del jugador va en Awake y no en Start?
        · "Resolvemos una referencia" (a quién seguimos), no intercambiamos información con otros objetos --> es el uso "de libro" de Awake.
        · Si lo hiciéramos en Start() también funcionaría, pero el patrón del proyecto (Heart, PlayerController, Coin) lo hace en Awake.
    */
    void Awake()
    {
        /*
        - ¿Qué hace exactamente "GameObject.FindGameObjectWithTag("Player")"?
            · Busca en la ESCENA el primer objeto ACTIVO cuyo TAG sea "Player" y devuelve su "GameObject".
            · Es la forma RECOMENDADA de resolver referencias por ROL (tag) frente a buscar por nombre:
              sigue funcionando aunque el objeto se renombrara en la jerarquía.
            · Si no existe ningún objeto con ese tag, devuelve "Nulo/Null" SILENCIOSAMENTE
              --> por eso la condicional siguiente lo comprueba ANTES de tocar ".transform".
        */
        // Buscamos el jugador por Tag (más robusto que por nombre)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        //· "playerObj != null": guard de seguridad ANTES de usar la referencia.
        //    --> Mismo patrón "guard + mensaje limpio" que los fixes de la sesión (SceneLoader, GameManager):
        //      si el jugador no está, no rompemos con NRE: registramos un error claro en la Consola.
        if (playerObj != null)
            //· "playerObj.transform": guardamos la REFERENCIA al "Transform" del jugador (su posición en el mundo),
            //    que es lo que "Update()" leerá cada frame para situar la cámara.
            cameraTarget = playerObj.transform;
        else
            //· "Debug.LogError(...)": imprime en la Consola de Unity que la cámara no tiene a quién seguir.
            Debug.LogError("CameraController: No se encontró objeto con Tag 'Player'.");
    }

    /*
    Unity invoca Update() una vez por frame (a frecuencia variable: una vez cada vez que el motor pinta un fotograma).
    - ¿Qué hace este método?
        · Pega la cámara al jugador DESPLAZADA por "cameraOffset", y después CORRE el resultado
          para que no salga de los límites "min/max" del nivel.
        · APUNTE HONESTO (sin tocar el código, pase SOLO comentarios): el patrón "de libro" para cámaras
          de seguimiento es "LateUpdate()" (se ejecuta DESPUÉS de que la escena se haya movido en ese frame,
          y evita que la cámara "persiga" con un frame de retraso). Aquí el proyecto usa "Update()"; queda documentado.
    */
    void Update()
    {
        //· Guard de salida: si en "Awake()" no se resolvió el jugador (ver el "LogError"), NO hay nada que seguir.
        //    --> Salir sin más evita la "NullReferenceException" que daría "cameraTarget.position" en el frame siguiente.
        if (cameraTarget == null) return;

        //· "cameraTarget.position + cameraOffset": la posición "ideal" de la cámara = posición del jugador + desplazamiento del Inspector.
        Vector3 desiredPosition = cameraTarget.position + cameraOffset;

        // Clamping de posición para que la cámara no salga del nivel
        //· "Mathf.Clamp(valor, min, max)": devuelve "valor" SI está entre "min" y "max"; si no, lo "apila" al límite más cercano.
        //    --> "minCameraPosition"/"maxCameraPosition" son "Vector2" (solo X e Y): por eso el clamp se hace componente a componente.
        float clampX = Mathf.Clamp(desiredPosition.x, minCameraPosition.x, maxCameraPosition.x);
        //· Idem para el eje Y, con los mismos límites del Inspector.
        float clampY = Mathf.Clamp(desiredPosition.y, minCameraPosition.y, maxCameraPosition.y);

        //· "transform.position = ...": movemos ESTE GameObject (la cámara) a la posición corregida.
        //    --> X e Y corregidos por "Clamp"; la Z se conserva la del "desiredPosition" (el plano fijo de cámara, -10 por defecto).
        transform.position = new Vector3(clampX, clampY, desiredPosition.z);
    }
}
