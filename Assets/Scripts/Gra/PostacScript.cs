using UnityEngine;
using UnityEngine.InputSystem;

public class PostacScript : MonoBehaviour
{     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public double hp = 100;
    public Rigidbody2D rb;
    public float moveCoefficient = 0.1f;
    public float maxSpeed = 5.0f;
    
    private Vector2 moveDirection;
    private float moveSpeed;

    private void move(float[] vector)
    {
        rb.linearVelocity += new Vector2(vector[0], vector[1]);
    }
    
    void Start()
    {
        
    }
    
    public void OnUp()
    {
        // move(new float[]{0, moveCoefficient});
        
        this.moveDirection += new Vector2(0, 1);
    }

    public void OnDown()
    {
        // move(new float[]{0, -moveCoefficient});
        
        this.moveDirection += new Vector2(0, -1);
    }

    public void OnLeft()
    {
        // move(new float[]{-moveCoefficient, 0});
        
        this.moveDirection += new Vector2(-1, 0);
    }

    public void OnRight()
    {
        // move(new float[]{moveCoefficient, 0});
        
        this.moveDirection += new Vector2(1, 0);
    }
    void FixedUpdate()
    {
        rb.linearVelocity += new Vector2(moveDirection.x, moveDirection.y) * moveCoefficient;
    }
}
