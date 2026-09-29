
using UnityEngine;

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
    - ¿Por qué "TakeDamage" es "public" y los demás métodos no?
        · Porque lo llama una clase EXTERNA: "PlayerController" lo invoca como "enemyScript.TakeDamage(_attackDamage)".
        · Si fuera "private" (la visibilidad por defecto de los métodos en C#), el compilador rechazaría la llamada desde "PlayerController".
    */
    public void TakeDamage(int damage)
    {
        //· "_actualHealth -= damage" es un atajo de "_actualHealth = _actualHealth - damage": descontamos el daño recibido de la vida corriente.
        _actualHealth -= damage;

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
