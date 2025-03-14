using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MultiInputSystem : MonoBehaviour
{   
    [SerializeField] private float _moveSpeed = 5.0f; 
    [SerializeField] public int LastDirection = 0;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isMovementEnabled = true;
    [SerializeField] private float stepIntenisty = 0.3f;
    [SerializeField] private AudioSource stepSound;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    
    private float lastStepTime = 0;
     

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!_isMovementEnabled) return;
        _rigidbody.velocity = _movementInput * _moveSpeed;
        LastDirection = GetDirection(_movementInput);
        if (lastStepTime + stepIntenisty < Time.time && _isMoving)
        {
            lastStepTime = Time.time;
            stepSound.Play();
            //Debug.Log("Step");
        }
    }
    
    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
    
    private int GetDirection(Vector2 movement)
    {
        // 0 = top, 1 = right, 2 = bottom, 3 = left
        if (movement != Vector2.zero) _isMoving = true;
        if (movement == Vector2.zero) _isMoving = false;
        if (movement == Vector2.zero) return LastDirection;
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        angle = (angle + 270) % 360; // Ensure angle is positive and 0 points to the top
        int direction = (int)Mathf.Round(angle / 90) % 4; // Adjusted to return 4 directions
        return direction;
    }

    
    public bool IsMoving()
    {
        return _isMoving;
    }
    
    public void enableMovement()
    {
        _isMovementEnabled = true;
    }
    public void disableMovement()
    {
        _isMovementEnabled = false;
        _rigidbody.velocity = Vector2.zero;
    }
}
