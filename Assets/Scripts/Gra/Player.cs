using System;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerInput playerInput;
    public GameObject bullet;
    private float bulletSpeed;
    public int hp = 5;
    public float parryColdown = 5f;
    public Zycie zycie;
    
    private LookingDirection _lookingDirection;
    
    public void OnFire(InputValue value)
    {   
        Vector2 playerXY = transform.position;
        Quaternion q = _lookingDirection.GetPlayerRotation();
        GameObject newBullet = Instantiate(bullet, playerXY, q);
        float rad = q.eulerAngles.z * Mathf.Deg2Rad;
        newBullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(bulletSpeed * Mathf.Cos(rad), bulletSpeed * Mathf.Sin(rad));
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Bullet(Clone)")
        {
            if (!collider.GetComponent<BulletScript>().isActive)
            {
                collider.GetComponent<BulletScript>().isActive = true;
                return;
            }
            zycie.TakeDamage(1.0f); 
            Destroy(collider.gameObject);
        }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        bulletSpeed = bullet.GetComponent<BulletScript>().bulletSpeed;
        _lookingDirection = GetComponent<LookingDirection>();
        zycie = GetComponent<Zycie>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
