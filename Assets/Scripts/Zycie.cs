using Events;
using UnityEngine;

public class Zycie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private const float maxHealth = 5f;
    [SerializeField] public float health = maxHealth;
    [SerializeField] private bool _isDead = false;
    
    void Start()
    {
    }

    public void resetHealth()
    {
        health = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        eventRegistry.healthEventOnHit(gameObject, damage);
        checkHealth();
    }

    public bool isDead()
    {
        return _isDead;
    }


    

    private void checkHealth()
    {
        if (health <= 0)
        {
            this._isDead = true;
            eventRegistry.healthEventOnDeath(gameObject);
        }
    }
    
}
