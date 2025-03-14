using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerInput playerInput;
    public GameObject bullet;
    public float bulletSpeed = 1;
    private Vector2 look;
    public int hp = 100;
    public float parryColdown = 5f;
    
    public void OnFire(InputValue value)
    {   
        Vector2 playerXY = transform.position;
        Vector2 endPosition;
        Debug.Log(playerInput.defaultControlScheme);
        if (playerInput.defaultControlScheme == "Keyboard&Mouse")
        {   
            Vector2 mouseScreenPosition = Input.mousePosition;
            endPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        else
        {
            endPosition = look;
        }

        float phiRad = Mathf.Atan2(endPosition.y - playerXY.y, endPosition.x - playerXY.x);
        float phi = phiRad * (180 / Mathf.PI);
        GameObject newBullet = Instantiate(bullet, playerXY, Quaternion.Euler(0, 0, phi));
        Debug.Log(playerXY.x + " " + playerXY.y + " " + endPosition.x + " " + endPosition.y);
        Debug.Log(phi);
        newBullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(bulletSpeed * Mathf.Cos(phiRad), bulletSpeed * Mathf.Sin(phiRad));
    }
    
    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>().normalized;
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
