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

    // Update is called once per frame
    void Update()
    {
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
    }

    void Attack()
    {
        _animator.SetTrigger("IsAttacking");

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundSensor.position, _sensorSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackHitBox.position, _hitBoxRadius);
    }
}
