using UnityEngine;

/*

Su ruta completa:
UnityEngine.InputSystem.InputSystem.actions["Move"]

CLASE: EnemySpawnClass (el molde) (Puede o no ser instanciable)
CAMPO: private int _contadorVidaEnemigo = 100; (variables guardadas dentro de la clase)
MÉTODO: RevivirEnemigo() (funciones guardadas dentro de la clase)
INSTANCIA: new EnemySpawnClass() (objetos creados a partir del molde)

com.unity.inputsystem   = Paquete base / Manifest.
DLLs Compiladas         = Assembly (Ficheros binarios "01100010101110").
UnityEngine.InputSystem = namespace (organización lógica de tipos, no existe en runtime).
.InputSystem            = clase estática (no instanciable, dentro del namespace anterior).
.actions                = propiedad estática (public static InputActionAsset, referencia el asset project-wide configurado en Project Settings).
["Move"]                = indexer (busca por nombre la acción en los action maps del asset y devuelve un InputAction).

*/

using UnityEngine.InputSystem;

//Declaramos clase pública "PlayerController" con herencia de "MonoBehaviour", es la clase de Unity que aporta de serie: ciclo de vida gestionado por el motor (Awake, Start, Update...)
//PlayerController sin heredar de MonoBehaviour es solo una clase C# normal que Unity ignora por completo.
public class PlayerController : MonoBehaviour
{
    //Definimos un campo privado "_rigidbody2D" es de tipo "Rigidbody2D", privado, valor por defecto nulo/null.
    private Rigidbody2D _rigidbody2D;
    //Definimos un campo privado "_animator" es de tipo "Animator", privado.
    private Animator _animator;

    //[SerializeField], permite que el acceso al campo pueda realizarse desde el inspector de unity sin comprometer el aislamiento del mismo campo al propio script que pertenece.

    //InputAction vive bajo el namespace UnityEngine.InputSystem
    //El 'using UnityEngine.InputSystem;' habilita la posibilidad de escribir los nombres cortos de los campos.

    //Definimos un campo privado "_maxHealth", de tipo "Entero" - Permite Números enteros. Con [SerializeField].
    [SerializeField]private int _maxHealth = 100;
    //Definimos un campo privado "_movementSpeed" de tipo "float" - Permite decimales. Con [SerializeField].
    [SerializeField]private float _movementSpeed = 4.5f;
    //Definimos un campo privado "_jumpHeight" de tipo "float" - Permite decimales. Con [SerializeField].
    [SerializeField]private float _jumpHeight = 2.0f;

    //Definimos un campo de tipo "InputAction" llamado "_attackAction", obtiene por defecto valor nulo/null. 
    private InputAction _attackAction;
    //Definimos un campo de tipo "InputAction" llamado "_jumpAction", obtiene por defecto valor nulo/null.
    private InputAction _jumpAction;
    //Definimos un campo de tipo "InputAction" llamado "_moveAction", obtiene por defecto valor nulo/null.
    private InputAction _moveAction;
    //Definimos un struct de tipo "Vector2" llamado "_moveInput", jamás puede ser null, obtiene por defecto valor (0,0).
    private Vector2 _moveInput;

    //Definimos un campo privado "_groundSensor", de tipo "Transform". Con [SerializeField].
    [SerializeField]private Transform _groundSensor;
    //Definimos un campo privado "_sensorSize", de tipo "float" - Permite decimales. Con [SerializeField].
    [SerializeField]private float _sensorSize = 1f;
    //Definimos un struct privado "_groundLayer", de etipo "LayerMask". Con [SerializeField].
    [SerializeField]private LayerMask _groundLayer; /// PENDIENTE MEJORAR DEFINICIÓN DE LAYER Y LAYERMASK
    //Definimos un campo privado "_attackDamage", de tipo "Entero" - Permite Números enteros. Con [SerializeField].
    [SerializeField]private int _attackDamage = 7;
    //Definimos un campo "_attackHitbox" de tipo "Transform". Con [SerializeField].
    [SerializeField]private Transform _attackHitBox;
    //Definimos un campo "_hitBoxRadius" de tipo "float" - Permite Decimales. Con [SerializeField].
    [SerializeField]private float _hitBoxRadius = 1f;

    // Declaramos el metodo "Awake".
    void Awake()
    {

        /*
        Awake se ejecuta antes que Start, "Use Awake to initialize variables or states before the application starts".
         Sin embargo, no existe especificación contractual explícita que prohíba intercambiar información en Awake.

        Antes de comenzar con "void Awake(){x}" deberíamos hacernos las siguientes preguntas:
        - ¿Por qué estas asignaciones van en Awake y no en otro sitio?
            · La documentación de MonoBehaviour.Awake que he auditado mediante la información oficial de unity:
              "Debes utilizar el Awake para ESTABLECER REFERENCIAS, y Start para intercambiar información"

        - ¿Qué ocurre realmente cuando solicitamos un "GetComponent<x>()"?
            · Mediante el "GetComponent" se busca UN componente de tipo "Rigidbody2D" ADJUNTO AL MISMO GameObject que lleva este script.
            · La ejecución del "GetComponent" devolverá el valor del resultado sobre la obtención de la REFERENCIA "GetComponent<Rigidbody2D>()" al campo "_rigidbody2D".
            · Es decir, "Solicitamos la obtención de la referencia "Rigidbody2D" y asignamos su valor al campo "_rigidbody2D".
            //Cambios 14:00 28092026.           
            -- · Si el objeto no tiene "Rigidbody2D", devolverá "Nulo/Null" porque así está especificado su contrato de retorno por parte de Unity.
            ++ · Se trata de una una referencia a un componente concreto, en caso de no encontrar el componente devolverá "Nulo/Null". 
            ++ · Si el GameObject no lleva Animator, _animator quedará en null silenciosamente — a diferencia del indexer de Input System, no habrá excepción que lo delate. -->
            ++ · --> Las fuentes de apoyo mencionan [RequireComponent(typeof(Animator))] como patrón para garantizar a nivel de compilación que el componente existe, evitando el null silencioso.

        - ¿Qué ocurre realmente cuando solicitamos un "InputSystem.actions["x"]"?
            · "InputSystem.actions", es la propiedad estática que devuelve el asset project-wide,  el indexer ["Move"] -->
                --> busca en sus action maps la acción con ese nombre y devuelve el objeto InputAction correspondiente.
            · La ejecución del "InputSystem.actions" devolverá el valor del resultado sobre la obtención de la REFERENCIA "InputSystem.actions["Move"]" al campo "_moveAction".
            · Es decir, "Solicitamos la obtención de la referencia "Move" y asignamos su valor al campo "_moveAction".
            · Si en el asset del "InputAction" no existiera una acción llamada "Move", esto lanzaría EXCEPCIÓN (A diferencia de GetComponent, que devuelve nulo/null).
        */

        //Asignamos el valor del resultado sobre la obtención de la REFERENCIA "GetComponent<Rigidbody2D>()" al campo "_rigidbody2D".
        _rigidbody2D = GetComponent<Rigidbody2D>();
        //Asignamos el valor del resultado sobre la obtención de la REFERENCIA "GetComponent<Animator>()" al campo "_animator".
        _animator = GetComponent<Animator>();

        /*
        - Tal y como he mencionado con anterioridad pero de forma algo más extensa:
            · InputSystem.actions es la propiedad estática que devuelve el asset project-wide (configurado en Project Settings, Input System Package), y el indexer ["Move"] -->
                --> busca en sus action maps la acción con ese nombre y devuelve el objeto InputAction correspondiente.

            · Es decir: "InputSystem.actions["Move"]" se trata de la referencia al objeto InputAction llamado "Move" dentro del asset project-wide.
            ·  Si en el asset no existiera una acción llamada "Move", esto lanzaría EXCEPCIÓN ( diferencia de GetComponent, que devuelve nulo/null).

        - ¿Qué es exactamente una "Excepción" vs un valor "Nulo/Null"?
            · Excepción es un fallo inmediato en el momento de la línea que la produce. La ejecución de esa línea se corta ahí mismo y salta a un manejador de errores, abortando el resto de Awake si nadie lo captura.
            · Nulo/Null es un valor devuelto en el momento de declarar un campo cuyo valor no haya sido asignado aún, es decir, no se quedan campos vacíos por no asignarles valor en el momento de su declaración sino que se "Rellenan por defecto" con "Nulo/Null".
        */

        //Recordatorio: Los siguientes campos son de tipo "InputAction", "Excepción" en vez de "Nulo/Null".
        //Asignamos el valor del resultado sobre la obtención de la REFERENCIA "InputSystem.actions["Move"]" al campo "_moveAction", 
        _moveAction = InputSystem.actions["Move"];
        //Asignamos el valor del resultado sobre la obtención de la REFERENCIA "InputSystem.actions["Jump"]" al campo "_jumpAction".
        _jumpAction = InputSystem.actions["Jump"];
        //Asignamos el valor del resultado sobre la obtención de la REFERENCIA "InputSystem.actions["Attack"]" al campo "_attackAction".
        _attackAction = InputSystem.actions["Attack"];

    }

    /*
    Unity invoca Update() una vez por FRAME: es decir, una vez cada vez que el motor pinta un fotograma, a una frecuencia variable.
    - ¿Por qué el input y las animaciones se leen en Update y no en FixedUpdate?
        · El input y las animaciones son cuestiones de "presentación": queremos que el personaje reaccione fotograma a fotograma, con la máxima sensibilidad.
        · FixedUpdate se ejecuta a un paso fijo (por defecto 0.02 s, "Time.fixedDeltaTime"), que no siempre coincide con el número de fotogramas; si dependiéramos de él, el input se sentiría entrecortado.
    */
    void Update()
    {
        //Asignamos al struct "_moveInput" el "Vector2" resultante de leer la acción "Move" mediante ReadValue<Vector2>().
        //· "ReadValue<T>()" devuelve el valor AGREGADO de todos los controles ligados a la acción en ese instante (el eje horizontal devuelve -1, 0 o 1).
        _moveInput = _moveAction.ReadValue<Vector2>();

        /*
        - ¿Qué hacemos exactamente con "transform.rotation = Quaternion.Euler(0, 180, 0)"?
            · "transform" es la propiedad del "MonoBehaviour" que expone el "Transform" del MISMO GameObject que lleva este script.
            · "rotation" es la propiedad que guarda la rotación del objeto como "Quaternion" (cuaternión: struct de 4 componentes x, y, z, w).
            · "Quaternion.Euler(0, 180, 0)" CONSTRUYE un cuaternión a partir de ángulos en GRADOS (0 en X, 180 en Y, 0 en Z): "dame la rotación de 180° sobre el eje Y".
            · En la práctica: giramos al personaje 180° sobre el eje Y para que "mire" hacia la izquierda; con (0, 0, 0) devolvemos la orientación original.
            · "_animator.SetBool("IsRunning", true)" busca en el Animator Controller el parámetro BOOL llamado "IsRunning" y lo activa; si el parámetro no existe, Unity avisa en la consola pero NO lanza excepción.
        */
        if(_moveInput.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _animator.SetBool("IsRunning", true);
        }
        else if(_moveInput.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }


        /*
        - ¿Qué hace exactamente "WasPressedThisFrame()"?
            · Es un método de la clase "InputAction" que devuelve true ÚNICAMENTE en el PRIMER frame en el que el control pasa a estar presionado.
            · Es la semántica de "pulsado justo ahora" que en el sistema de input antiguo hacíamos a mano con "Input.GetKeyDown(...)".
            · Diferencia con "GetPressed()": este último devuelve true DURANTE TODO el tiempo que el control siga pulsado.
            · "&& IsGrounded()": solo permitimos la acción si el sensor detecta suelo, evitando saltos dobles y ataques en el aire.
        */
        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        if(_attackAction.WasPressedThisFrame() && IsGrounded())
        {
            Attack();
        }

        //· "_animator.SetBool("IsJumping", !IsGrounded())": cada frame sincronizamos el parámetro "IsJumping" del Animator con el estado real (el "!" invierte el bool).
        _animator.SetBool("IsJumping", !IsGrounded());
    }

    /*
    Unity invoca FixedUpdate() a un PASO FIJO (por defecto 0.02 s, "Time.fixedDeltaTime" en Project Settings → Time), es decir, 50 veces por segundo independientemente de la tasa de fotogramas.
    - ¿Por qué el movimiento del Rigidbody va en FixedUpdate y no en Update?
        · La simulación de la física 2D (gravedad, colisiones, fuerzas) solo "avanza" dentro de FixedUpdate.
        · Si modificáramos la velocidad en Update, estaríamos peleando contra la simulación y el resultado dependería de la tasa de refresco.
    */
    void FixedUpdate()
    {
        //· "linearVelocity" es la propiedad del "Rigidbody2D" que fija su velocidad DIRECTAMENTE (no suma una fuerza).
        //· "new Vector2(_moveInput.x * _movementSpeed, _rigidbody2D.linearVelocity.y)":
        //    --> X: multiplicamos el eje del input por la velocidad configurada en el Inspector.
        //    --> Y: conservamos la componente vertical actual (la que lleva la gravedad); si aquí pusiéramos "0" anularíamos la caída.
        _rigidbody2D.linearVelocity = new Vector2(_moveInput.x * _movementSpeed, _rigidbody2D.linearVelocity.y);
    }

    /*
    - ¿De dónde saca su valor la fórmula "Mathf.Sqrt(_jumpHeight * -2 * Physics2D.gravity.y)"?
        · "Physics2D.gravity" es la propiedad estática con la gravedad del mundo 2D (Vector2, por defecto (0, -9.81)).
        · De la cinemática clásica: h = v² / (2g) --> despejando, v = sqrt(2·g·h).
        · Como "gravity.y" es negativo, usamos "_jumpHeight * -2 * Physics2D.gravity.y" para que el producto sea POSITIVO y la raíz tenga sentido.
        · Resultado: el valor de "_jumpHeight" (2.0 por defecto) determina la ALTURA MÁXIMA del salto, no la fuerza bruta.
    */
    void Jump()
    {
        //· "Vector2.up" es el struct (0, 1): dirección vertical hacia arriba.
        //· "AddForce(fuerza, ForceMode2D.Impulse)": aplica un CAMBIO INSTANTÁNEO en el momento lineal (equivalente a un cambio instantáneo de velocidad), sin depender del delta-time.
        _rigidbody2D.AddForce(Vector2.up * Mathf.Sqrt(_jumpHeight * -2 * Physics2D.gravity.y), ForceMode2D.Impulse);
    }

    void Attack()
    {
        //· "SetTrigger("IsAttacking")": activa el parámetro "trigger" del Animator.
        //    --> A diferencia del Bool, un Trigger es un estado de UN SOLO FRAME: el Animator lo "consume" para la transición y lo vuelve a false automáticamente.
        _animator.SetTrigger("IsAttacking");

        //· "Physics2D.OverlapCircleAll(posición, radio)" es una CONSULTA pura: devuelve un "array" (posiblemente vacío) con TODOS los "Collider2D" que se solapan con el círculo.
        //    --> No empuja ni mueve nada, solo "pregunta" qué hay dentro.
        //    --> Diferencia con "Physics2D.OverlapCircle(...)": este devuelve solo el PRIMER colisionador encontrado.
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(_attackHitBox.position, _hitBoxRadius);

        //· Recorremos colisionador a colisionador los que la consulta ha encontrado.
        foreach (Collider2D enemy in colliders2D)
        {
            //· "enemy.gameObject.layer == 7": comprobamos que el colisionador pertenece a un GameObject situado en la capa 7 (enemigos).
            //    --> OJO: comparar con el número "7" a pelo es frágil; si reordenamos las capas en Tag Manager, el código dejaría de funcionar en silencio.
            if(enemy.gameObject.layer == 7)
            {
                //· "GetComponent<Mimik>()": mismo contrato que en Awake, puede devolver nulo/null si el GameObject no lleva el componente.
                //    --> En ese caso, la siguiente línea lanzaría "NullReferenceException".
                Mimik enemyScript = enemy.GetComponent<Mimik>();
                //· Llamamos al método "TakeDamage" de nuestra propia clase "Mimik" pasándole el daño configurado en el Inspector.
                enemyScript.TakeDamage(_attackDamage);
            }
        }
    }

    /*
    - ¿Qué hace exactamente este método?
        · "Pregunta" al sistema de física si hay suelo debajo del sensor "_groundSensor".
        · Se usa como condición del salto, del ataque y de la animación "IsJumping".
    */
    bool IsGrounded()
    {
        //· Misma consulta que en "Attack()": devuelve un "array" (posiblemente vacío) con los colisionadores dentro del círculo.
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(_groundSensor.position, _sensorSize);

        //· Recorremos los colisionadores encontrados.
        foreach (Collider2D item in colliders2D)
        {
            //· Si ALGUNO de los colisionadores tocados está en la capa 6 (suelo), el jugador "toca suelo".
            if(item.gameObject.layer == 6)
            {
                return true;
            }
        }
        //· Si hemos terminado el recorrido sin tocar ningún colisionador de suelo --> el jugador está en el aire.
        return false;
    }

    /*
    Unity invoca OnDrawGizmos() en la vista de ESCENA del editor (no aparece en la vista de juego), mientras el editor esté abierto.
    - Sirve para visualizar elementos "invisibles" del gameplay (sensores, hitboxes) sin tocar la lógica del juego.
    */
    void OnDrawGizmos()
    {
        //· "Gizmos.color" fija el color que usarán los DIBUJOS posteriores.
        Gizmos.color = Color.red;
        //· "Gizmos.DrawWireSphere(posición, radio)" dibuja una esfera "alambre" en la vista de escena; nos sirve para ver la extensión real del sensor de suelo.
        Gizmos.DrawWireSphere(_groundSensor.position, _sensorSize);

        Gizmos.color = Color.yellow;
        //· Idem para la hitbox del ataque.
        Gizmos.DrawWireSphere(_attackHitBox.position, _hitBoxRadius);
    }
}


/*

Apuntes 30092026 0904h

Un Array se declara igual que una variable pero
public int noArrayNum;
para acceder:
noArrayNum = 12;
vs
public int[] arrayNum;
Para acceder a una array:
arrayNum[0] = 12;


Luego puedo tener listas,
Si quiero un inventario fijo utilizo un array de 10.
Si quieor

ro un inventario por peso, hago una lista con la condicional de peso.

*/

/*
apuntes profe:
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private AudioSource _playerAudioSource;

    private InputAction _pauseAction;
    private InputAction _attackAction;
    private InputAction _jumpAction;
    private InputAction _moveAction;
    private Vector2 _moveInput;

    //int para guardar la vida maxima
    [SerializeField]private int _maxHealth = 100;
    [SerializeField]private int _actualHealth;
    [SerializeField]private float _movementSpeed = 4.5f;
    [SerializeField]private float _jumpHeight = 2;    


    [SerializeField]private Transform _groundSensor;
    [SerializeField]private float _sensorSize = 1;
    [SerializeField]private LayerMask _groundLayer;

    [SerializeField]private int _attackDamage = 7;
    [SerializeField]private Transform _attackHitBox;
    [SerializeField]private float _hitBoxRadius = 1f;

    [SerializeField]private AudioClip _jumpSound;
    [SerializeField]private AudioClip _attackSound;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerAudioSource = GetComponent<AudioSource>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _attackAction = InputSystem.actions["Attack"];
        _pauseAction = InputSystem.actions["Pause"];
    }

    void Start()
    {
        //_actualHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(_pauseAction.WasPressedThisFrame())
        {
            GameManager.Instance.Pause();
        }

        if(GameManager.Instance.IsPaused())
        {
            return;
        }

        _moveInput = _moveAction.ReadValue<Vector2>();

        if(_moveInput.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _animator.SetBool("IsRunning", true);
        }
        else if(_moveInput.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }


        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        if(_attackAction.WasPressedThisFrame() && IsGrounded())
        {
            Attack();
        }

        _animator.SetBool("IsJumping", !IsGrounded());
    }

    void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = new Vector2(_moveInput.x * _movementSpeed, _rigidbody2D.linearVelocity.y);
    }

    void Jump()
    {
        _rigidbody2D.AddForce(Vector2.up * Mathf.Sqrt(_jumpHeight * -2 * Physics2D.gravity.y), ForceMode2D.Impulse);
        PlaySFX(_jumpSound);
    }

    void Attack()
    {
        _animator.SetTrigger("IsAttacking");

        PlaySFX(_attackSound, 0.7f);

        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(_attackHitBox.position, _hitBoxRadius);

        foreach (Collider2D enemy in colliders2D)
        {
            if(enemy.gameObject.layer == 7)
            {
                Mimik enemyScript = enemy.GetComponent<Mimik>();
                enemyScript.TakeDamage(_attackDamage);
            }
        }
    }

    bool IsGrounded()
    {
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(_groundSensor.position, _sensorSize);

        foreach (Collider2D item in colliders2D)
        {
            if(item.gameObject.layer == 6)
            {
                return true;
            }
        }
        return false;
    }

    void PlaySFX(AudioClip clip, float volume = 1)
    {
        _playerAudioSource.PlayOneShot(clip, volume);
    }

    public void AddHealth(int healAmaount)
    {
        _actualHealth += healAmaount;

        if(_actualHealth > _maxHealth)
        {
            _actualHealth = _maxHealth;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundSensor.position, _sensorSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackHitBox.position, _hitBoxRadius);
    }
}

*/