using UnityEngine;

//Declaramos clase pública "DeathSensor" con herencia de "MonoBehaviour" (mismo arranque que "Heart" o "Mimik").
//Es la zona de muerte por CAÍDA del nivel: un "sensor" que avisa cuando el jugador cae fuera de las plataformas.
//· El GameObject "DeathSensor" (creado en "Nivel 1") lleva un BoxCollider2D marcado como TRIGGER (verificado en la escena: "m_IsTrigger: 1"),
//  colocado bajo el nivel como zona de muerte.
public class DeathSensor : MonoBehaviour
{
    /*
    Unity invoca OnTriggerEnter2D(Collider2D other) cuando el colisionador TRIGGER de ESTE objeto
    se SOLAPA con otro colisionador (el Rigidbody2D/CapsuleCollider2D del jugador), y nos pasa
    por parámetro el colisionador con el que se ha solapado.
    - ¿Por qué "OnTriggerEnter2D" y no "OnCollisionEnter2D"?
        · Nuestro colisor es un TRIGGER (m_IsTrigger: 1): no empuja, solo "avisa" del solape
          --> el callback que Unity invoca es "OnTriggerEnter2D" (patrón de Heart / Coin / Star: "OnTriggerEnter2D(Collider2D other)").
        · "OnCollisionEnter2D" (el patrón de Mimik / Spikes del Zombie) solo se dispara con colisionadores SÓLIDOS
          (m_IsTrigger: 0). Si lo pusiéramos aquí, Unity jamás lo invocaría y la caída no mataría.
        · Mismo razonamiento que el comentario de "OnTriggerEnter2D" de Heart ("Trigger NO empuja ni bloquea: solo avisa").
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        - ¿Qué hace "other.CompareTag("Player")"?
            · Comprueba que el TAG del objeto que se solapa con el sensor es "Player" (el jugador).
            · Es la forma RECOMENDADA de comprobar tags (mismo patrón que Heart / Coin / Star / Mimik).
            · Sin esta comprobación, el sensor "dispararía" al solaparse con cualquier cosa que tenga colisionador
              (p. ej. una moneda o una estrella que pasara por la zona) --> y solo nos interesa la CAÍDA DEL JUGADOR.
        */
        if(other.CompareTag("Player"))
        {
            /*
            - Guard de "Instance": si no hubiera ningún GameManager en la escena, "Instance" sería "null"
              --> en lugar de "NullReferenceException", salta un aviso legible en la Consola (mismo patrón que los guards del proyecto).
            */
            if(GameManager.Instance != null)
            {
                /*
                - ¿Por qué "PlayerTakeDamage(999)" y no "GameOver()" directo?
                    · Respeta la CADENA DE DAÑO existente: "PlayerTakeDamage" resta la vida, mueve la barra
                      ("UpdateHealthBar") y, al llegar a 0 (o quedar negativa), lanza "GameOver()" --> escena "GameOver".
                    · 999 es un "daño instantáneo" (instakill): con la vida máxima del jugador (100) siempre sobra para caer a 0.
                    · "GameOver()" se encarga del resto: marca "_gameOver" (evita doble cambio de escena),
                      restaura "Time.timeScale" y carga la escena "GameOver" a través de "SceneLoader".
                    · Si llamáramos a "GameOver()" directo, la barra de vida NO bajaría a 0 (saltaríamos la UI),
                      y si algún día "GameOver" hiciera más cosas, perderíamos ese paso intermedio.
                */
                GameManager.Instance.PlayerTakeDamage(999);
            }
            else
            {
                //· Misma "higiene de errores" que en Mimik: aviso legible en la Consola en lugar de una excepción críptica.
                Debug.LogError("DeathSensor: GameManager no encontrado en la escena; no puedo provocar Game Over.");
            }
        }
    }
}

/*
APUNTES CLASE:
using UnityEngine;

public class DeathSensor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.PlayerTakeDamage(999);
            }
            else
            {
                Debug.LogError("DeathSensor: GameManager no encontrado en la escena; no puedo provocar Game Over.");
            }
        }
    }
}
*/
