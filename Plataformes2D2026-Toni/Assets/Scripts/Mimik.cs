
using UnityEngine;

public class Mimik : MonoBehaviour
{
    [SerializeField]private int _maxHealth = 20;
    private int _actualHealth;

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