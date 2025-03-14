using UnityEngine;
using UnityEngine.InputSystem;


public class LightFollow : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject LightToRotate;
    public float angleOffset = 90.0f;
    public float rotationSmoothTime = 20.0f;
    
    private Vector2 look;
    
    private Vector2 _movementInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.defaultControlScheme != "Keyboard&Mouse")
        {
            if (look != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg);
                LightToRotate.transform.rotation =
                    Quaternion.Lerp(LightToRotate.transform.rotation, targetRotation, rotationSmoothTime * Time.deltaTime);
            }
        }
        else
        {
            // calculate angle beetwen player and mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg + angleOffset);
            LightToRotate.transform.rotation = Quaternion.Lerp(LightToRotate.transform.rotation, targetRotation, rotationSmoothTime * Time.deltaTime);
        }
    }
    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>().normalized;
    }
    
    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
}
