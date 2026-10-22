using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    private Transform cameraTarget;
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -10); 

    [Header("Clamping Bounds")]
    public Vector2 minCameraPosition;
    public Vector2 maxCameraPosition;

    void Awake()
    {
        // Buscamos el jugador por Tag (más robusto que por nombre)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            cameraTarget = playerObj.transform;
        else
            Debug.LogError("CameraController: No se encontró objeto con Tag 'Player'.");
    }

    void Update()
    {
        if (cameraTarget == null) return;

        Vector3 desiredPosition = cameraTarget.position + cameraOffset;

        // Clamping de posición para que la cámara no salga del nivel
        float clampX = Mathf.Clamp(desiredPosition.x, minCameraPosition.x, maxCameraPosition.x);
        float clampY = Mathf.Clamp(desiredPosition.y, minCameraPosition.y, maxCameraPosition.y);

        transform.position = new Vector3(clampX, clampY, desiredPosition.z);
    }
}
