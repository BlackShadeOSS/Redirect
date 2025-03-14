using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.LowLevel;

public class LookingDirection : MonoBehaviour
{
    private Transform playerTransform = null;
    private PlayerInput playerInput = null;
    public Quaternion playerRotation = Quaternion.identity;

    private bool _isGamepad = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput == null) { return; }

        if (!playerInput.defaultControlScheme.Contains("Keyboard"))
        {
            _isGamepad = true;
        }
        else
        {
            _isGamepad = false;
        }
        
        // get rotation
        if (!_isGamepad)
        {
            // calculate by mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(mousePos.y - playerTransform.position.y, mousePos.x - playerTransform.position.x) * Mathf.Rad2Deg);
            this.playerRotation = targetRotation; 
        }
    }
    
    void OnLook(InputValue value)
    {
        Vector2 lookInput = value.Get<Vector2>();
    
        if (_isGamepad)
        {
            float angle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg;
            playerRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public Quaternion GetPlayerRotation()
    {
        return playerRotation;
    }

    public bool isGamepad()
    {
        return _isGamepad;
    }
}
