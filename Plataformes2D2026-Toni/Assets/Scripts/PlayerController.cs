//Añadimos namespace UnityEngine: UnityEngine es un espacio de nombres dentro de los assemblies (DLLs) que trae Unity.

using UnityEngine;

// Su ruta completa:
//UnityEngine.InputSystem.InputSystem.actions["Move"]
// └────namespace────┘ └─clase─┘

//Añadimos namespace anidado "InputSystem" del namespace "UnityEngine".

//Se trata de dos capas distintas:
//El paquete InputSystem (Cuando nos referimos a "paquete" realmente estamos haciendo referencia a "com.unity.inputsystem" mediante el namespace, InputSystem es una dependencia instalable, 'Package Manager').
//El namespace (Ruta de nombres que se vuelve visible al compilador).

/*
com.unity.inputsystem  ←  el PAQUETE (el disco físico que compras/conectas)
   └─ DLLs compiladas  ←  el ASSEMBLY (el disco)
        └─ namespace UnityEngine.InputSystem  ←  la RUTA de directorios
             └─ clase InputSystem  ←  un ARCHIVO dentro de esa ruta
                  └─ propiedad actions  ←  contenido del archivo, diccionario de InputActions
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

    //Definimos un campo privado "_maxHealth", de tipo "Entero - Permite Números enteros (Lo he encontrado LUMO :D)", mediante "SerializeField" el campo pasará a ser visible en el inspector de Unity, el valor del inspector sobreescribirá el del código en caso de que la escena guarde un valor serializado para ese campo.
    [SerializeField]private int _maxHealth = 100;
    //Definimos un campo privado "_movementSpeed" de tipo "float - Permite decimales, mediante "SerializeField" el campo pasará a ser visible en el inspector de Unity, el valor del inspector sobreescribirá el del código en caso de que la escena guarde un valor serializado para ese campo.
    [SerializeField]private float _movementSpeed = 4.5f;
    //Definimos un campo privado "_jumpHeight" de tipo "float - Permite decimales, mediante "SerializeField" el campo pasará a ser visible en el inspector de Unity, el valor del inspector sobreescribirá el del código en caso de que la escena guarde un valor serializado para ese campo.
    [SerializeField]private float _jumpHeight = 2.0f;

    //Ejemplo casos anteriores:  "[SerializeField]private int _maxHealth = 100;" != "public int _maxHealth = 100;" SerializeField, permite que el acceso al campo pueda realizarse desde el inspector de unity sin comprometer el aislamiento del mismo campo al propio script que pertenece.

    //InputAction vive bajo el namespace UnityEngine.InputSystem
    //El 'using UnityEngine.InputSystem;' habilita la posibilidad de escribir los nombres cortos de los campos.
    //Definimos un campo de tipo "InputAction" llamado "_attackAction", obtiene por defecto valor nulo/null. 
    private InputAction _attackAction;
    //Definimos un campo de tipo "InputAction" llamado "_jumpAction", obtiene por defecto valor nulo/null.
    private InputAction _jumpAction;
    //Definimos un campo de tipo "InputAction" llamado "_moveAction", obtiene por defecto valor nulo/null.
    private InputAction _moveAction;
    //Definimos un struct de tipo "Vector2" llamado "_moveInput", jamás puede ser null, obtiene por defecto valor (0,0).
    private Vector2 _moveInput;

    [SerializeField]private Transform _groundSensor;
    [SerializeField]private float _sensorSize = 1;
    [SerializeField]private LayerMask _groundLayer;

    [SerializeField]private int _attackDamage = 7;
    [SerializeField]private Transform _attackHitBox;
    [SerializeField]private float _hitBoxRadius = 1f;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
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
