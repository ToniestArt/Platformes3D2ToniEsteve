
using UnityEngine;
using UnityEngine.UI;

//Declaramos clase pública "Mimik" con herencia de "MonoBehaviour", es la clase de Unity que aporta de serie: ciclo de vida gestionado por el motor (Awake, Start, Update...).
//Mimik sin heredar de MonoBehaviour sería solo una clase C# normal que Unity ignoraría por completo.
public class Mimik : MonoBehaviour
{
    //Definimos un campo privado "_maxHealth", de tipo "Entero" - Permite Números enteros. Con [SerializeField].
    //· "_maxHealth" es la vida máxima configurada desde el Inspector, es el "techo" desde el que el enemigo arranca.
    [SerializeField]private int _maxHealth = 20;
    //Definimos un campo privado "_actualHealth", de tipo "Entero" - Permite Números enteros.
    //· "_actualHealth" guarda la vida CORRIENTE del enemigo, es el valor que "TakeDamage" va descontando.
    private int _actualHealth;
    //Definimos un campo privado "_healthBar", de tipo "Slider". Con [SerializeField].
    //--> Es la BARRA DE VIDA del enemigo (el Slider que Toni creó bajo el Canvas hijo del Mimik).
    //    · Su rango va de 0 a 1 (verificado en la escena: "MinValue: 0", "MaxValue: 1").
    //    · Lo actualizamos en "TakeDamage()" para que baje visualmente con cada golpe.
    //    · Sin [SerializeField] no aparecería en el Inspector y no podríamos asignarlo/auditarlo.
    [SerializeField] private Slider _healthBar;

    /*
    Unity invoca Awake() UNA SOLA VEZ cuando el GameObject se activa, ANTES que Start().
    - ¿Por qué la asignación del Slider va aquí y no en Start?
        · "Asignamos referencias" (GetComponent*), no intercambiamos información con otros objetos --> es el uso "de libro" de Awake.
        · Mismo patrón que en Heart / Coin / Star / CameraController: referencias en Awake, estado en Start.
    */
    void Awake()
    {
        /*
        - ¿Qué hace "GetComponentInChildren<Slider>()"?
            · Busca el primer componente "Slider" en ESTE GameObject O en CUALQUIERA DE SUS HIJOS (a cualquier profundidad).
            · Nuestra barra está DOS niveles abajo: Mimik -> Canvas -> Slider --> "GetComponent" solo miraría al Mimik
              (y no encontraría nada); "GetComponentInChildren" sí la encuentra.
            · Si no hubiera ningún Slider en la rama, devolvería "Nulo/Null" SILENCIOSAMENTE
              --> por eso en "TakeDamage" guardamos con "_healthBar != null" antes de tocar ".value".
        */
        //Asignamos al campo "_healthBar" el Slider que hay en la rama de hijos del Mimik.
        _healthBar = GetComponentInChildren<Slider>();
    }

    /*
    Unity invoca Start() una única vez, ANTES del primer Update(), una vez que el MonoBehaviour ya está completamente creado.
    - ¿Por qué inicializamos "_actualHealth" aquí y no en la declaración del campo?
        · "_maxHealth" es editable desde el Inspector; si hiciéramos "int _actualHealth = _maxHealth;" en la declaración, C# copiaría el valor POR DEFECTO (20) del momento de instanciación, antes de que Unity escriba el valor que Toni haya puesto en el Inspector.
        · Al copiarlo en Start, "_actualHealth" refleja siempre el "_maxHealth" REAL que Unity ha asignado al componente.
    */
    void Start()
    {
        //Asignamos al campo "_actualHealth" el valor del campo "_maxHealth": el enemigo arranca con su vida máxima completa.
        _actualHealth = _maxHealth;
    }

    //Unity invoca Update() una vez por frame (una vez cada vez que el motor pinta un fotograma, a frecuencia variable).
    //· En "Mimik" queda vacío por ahora: el enemigo no tiene comportamiento propio, solo recibe daño. Si añadiera IA (patrulla, detección...), iría aquí.
    void Update()
    {
        
    }

    /*
    Unity invoca OnCollisionEnter2D(Collision2D collision) cuando el colisionador de ESTE objeto (un BoxCollider2D
    SÓLIDO: en la escena "m_IsTrigger: 0") entra EN CONTACTO por primera vez con otro colisionador sólido.
    - ¿Por qué "Collision" y no "Trigger"?
        · Los "trigger" NO empujan y disparan "OnTriggerEnter2D" (el patrón de Heart / Coin / Star: recoger sin interferir).
        · Nuestro colisionador es SÓLIDO (empuja al jugador) --> el callback que Unity invoca es "OnCollisionEnter2D"
          y el parámetro es "Collision2D" (no "Collider2D": aquí llega toda la colisión, con el objeto por "collision.gameObject").
        · Mismo patrón que "Spikes.cs" del proyecto Zombie (lo que en el Zombie mataba al jugador, aquí le quita vida).
    */
    void OnCollisionEnter2D(Collision2D collision)
    {
        //· "CompareTag("Player")": solo el JUGADOR recibe daño del contacto (misma comprobación que Heart / Coin / Star).
        //    --> Sin ella, el mímico se "auto-dañaría" al pisar suelo o rozar otra cosa que tenga colisionador.
        if(collision.gameObject.CompareTag("Player"))
        {
            /*
            - Guard de "Instance": si no hubiera ningún GameManager en la escena, "Instance" sería "null"
              --> en lugar de "NullReferenceException", salta un aviso legible en la Consola (mismo patrón que los guards del proyecto).
            */
            if(GameManager.Instance != null)
            {
                //· 10 de daño: la MISMA cantidad que cura el Heart (10) --> simetría: un toque de mímico = un corazón perdido.
                //    → "PlayerTakeDamage" resta la vida, mueve la barra ("UpdateHealthBar") y lanza "GameOver()" si llega a 0.
                GameManager.Instance.PlayerTakeDamage(10);
            }
            else
            {
                Debug.LogError("Mimik: GameManager no encontrado en la escena; no puedo hacer daño al jugador.");
            }
        }
    }

    /*
    - ¿Por qué "TakeDamage" es "public" y los demás métodos no?
        · Porque lo llama una clase EXTERNA: "PlayerController" lo invoca como "enemyScript.TakeDamage(_attackDamage)".
        · Si fuera "private" (la visibilidad por defecto de los métodos en C#), el compilador rechazaría la llamada desde "PlayerController".
    */
    public void TakeDamage(int damage)
    {
        //· "_actualHealth -= damage" es un atajo de "_actualHealth = _actualHealth - damage": descontamos el daño recibido de la vida corriente.
        _actualHealth -= damage;

        /*
        - ¿Qué hace esta línea con la barra?
            · "_actualHealth / _maxHealth": la FRACCIÓN de vida que queda (p. ej. 14 / 20 = 0.7).
            · "(float)" delante: en C# "int / int" da "int" (14 / 20 = 0, ¡se cortaría por abajo!);
              al convertir el numerador a "float" la división sale decimal (0.7) --> el Slider lo entiende
              porque su rango es 0-1 (verificado en la escena).
            · Guard "_healthBar != null": si el Slider no existiera (o se eliminara), "GetComponentInChildren"
              devolvió nulo en silencio --> sin el guard saltaría "NullReferenceException" en ".value".
            · Si el golpe mata (queda negativo), el Slider RECORTA SOLO al mínimo (0): no hace falta "Mathf.Clamp".
        */
        if(_healthBar != null)
        {
            _healthBar.value = (float)_actualHealth / _maxHealth;
        }

        /*
        - ¿Qué pasa si el daño sobrepasa la vida? (Ej.: vida 5, daño 7 --> "_actualHealth" = -2)
            · No es un problema para esta lógica: la condición "<= 0" captura también los valores negativos, el enemigo muere igualmente.
            · Lo que "NO" está resuelto aquí: el daño residual (los -2 "excedentes") se pierde; si el Mimik defendiera escudos o reflejara daño, habría que guardarlo.
        */
        if(_actualHealth <= 0)
        {
            //· Llamamos al método "Die()" de nuestra propia clase: vida a cero o por debajo --> el enemigo muere.
            Die();
        }
    }

    /*
    - ¿Qué hace exactamente "Destroy(gameObject)"?
        · "gameObject" es la propiedad del "MonoBehaviour" que referencia el MISMO GameObject que lleva este script (el enemigo).
        · "Destroy(objeto)" pide a Unity que elimine ese GameObject al FINAL del frame actual: no desaparece en el mismo instante de la llamada, Unity lo programa para la limpieza de fin de frame.
        · Al destruirse el GameObject, se destruyen con él sus componentes (colisor, sprite, este mismo script), sus hijos y deja de existir para la física y las consultas.
        · Diferencia con "DestroyImmediate(...)": este último elimina el objeto al momento de la llamada, pero solo debe usarse desde el editor (o fuera del juego), nunca dentro del flujo normal del juego.
    */
    void Die()
    {
        //· Eliminamos (programada al final del frame) el GameObject del enemigo.
        Destroy(gameObject);
    }
}


/*
APUNTES CLASE:
using UnityEngine;

public class Mimik : MonoBehaviour
{
    [SerializeField]private int _maxHealth = 20;
    private int _actualHealth;
    [SerializeField] private Slider _healthBar;

    void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _actualHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _actualHealth -= damage;

        if(_healthBar != null)
        {
            _healthBar.value = (float)_actualHealth / _maxHealth;
        }

        if(_actualHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
*/