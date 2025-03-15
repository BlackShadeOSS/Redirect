using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerInput playerInput;
    public GameObject bullet;
    private float bulletSpeed;
    public int hp = 100;
    public float parryColdown = 5f;
    
    private LookingDirection _lookingDirection;
    
    public void OnFire(InputValue value)
    {   
        Vector2 playerXY = transform.position;
        Quaternion q = _lookingDirection.GetPlayerRotation();
        GameObject newBullet = Instantiate(bullet, playerXY, q);
        float rad = q.z * 180;
        newBullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(bulletSpeed * Mathf.Cos(rad), bulletSpeed * Mathf.Sin(rad));
        Debug.Log("Jesok: " + q.z);
    }
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        bulletSpeed = bullet.GetComponent<BulletScript>().bulletSpeed;
        _lookingDirection = GetComponent<LookingDirection>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
