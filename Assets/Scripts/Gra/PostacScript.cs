using UnityEngine;

public class PostacScript : MonoBehaviour
{     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public double hp = 100;
    public Rigidbody2D rb;
    public float moveCoefficient = 0.1f;

    private void move(float[] vector)
    {
        rb.linearVelocity += new Vector2(vector[0], vector[1]);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            move(new float[]{0, moveCoefficient});
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move(new float[]{moveCoefficient, 0});
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            move(new float[]{-moveCoefficient, 0});
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move(new float[]{0, -moveCoefficient});
        }
    }
}
