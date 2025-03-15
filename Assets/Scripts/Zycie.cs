using Events;
using UnityEngine;

public class Zycie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private const float maxHealth = 5f;
    [SerializeField] private float health = maxHealth;
    [SerializeField] private bool _isDead = false;
    
    private eventRegistry _eventRegistry;
    void Start()
    {
        _eventRegistry = GetComponent<eventRegistry>();
    }

    public void resetHealth()
    {
        health = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        _eventRegistry.healthEventOnHit(damage);
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
            _eventRegistry.healthEventOnDeath();
        }
    }
    
}
