using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] public int LastDirection = 0;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isMovementEnabled = true;
    [SerializeField] private float stepIntensity = 0.3f;
    // [SerializeField] private AudioSource stepSound;
    
    [Header("Animation References")]
    [SerializeField] private AnimationClip[] movementAnimations = new AnimationClip[5]; // Array for movement animations
    [SerializeField] private Sprite[] idleSprites = new Sprite[5]; // Array for idle sprites
    [Tooltip("If true, will use direct animation arrays instead of Animator")]
    [SerializeField] private bool useDirectAnimations = false;
    
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Animation _animationComponent;
    
    private float lastStepTime = 0;
    private bool _facingRight = false;
    private int _currentAnimationIndex = 0;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _animationComponent = GetComponent<Animation>();
        
        if (useDirectAnimations && _animationComponent != null)
        {
            // Add all animation clips to the Animation component
            for (int i = 0; i < movementAnimations.Length; i++)
            {
                if (movementAnimations[i] != null)
                {
                    _animationComponent.AddClip(movementAnimations[i], movementAnimations[i].name);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isMovementEnabled) return;
        
        _rigidbody.linearVelocity = _movementInput * _moveSpeed;
        LastDirection = GetDirection(_movementInput);
        
        // Handle sprite flipping based on direction
        UpdateFacingDirection(LastDirection);
        
        // Handle animations based on movement and direction
        UpdateAnimation(LastDirection);
        
        // Handle footstep sounds
        if (lastStepTime + stepIntensity < Time.time && _isMoving)
        {
            lastStepTime = Time.time;
            // stepSound.Play();
            // Debug.Log("Step");
        }
    }
    
    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
    
    private int GetDirection(Vector2 movement)
    {
        // Update movement state
        _isMoving = movement != Vector2.zero;
        
        // If not moving, keep the last direction
        if (!_isMoving) return LastDirection;
        
        // Calculate angle (in degrees) of movement
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        
        // Normalize angle to 0-360 range
        angle = (angle + 360) % 360;
        
        // Convert angle to 8 directions (0-7)
        // 0 = North, 1 = Northeast, 2 = East, 3 = Southeast,
        // 4 = South, 5 = Southwest, 6 = West, 7 = Northwest
        int direction = Mathf.RoundToInt(angle / 45) % 8;
        
        return direction;
    }
    
    private void UpdateFacingDirection(int direction)
    {
        // Determine if character should face right or left
        if (direction == 1 || direction == 2 || direction == 3)
        {
            _facingRight = true;
            _spriteRenderer.flipX = true; // Flipping sprites for right-facing directions
        }
        else if (direction == 5 || direction == 6 || direction == 7)
        {
            _facingRight = false;
            _spriteRenderer.flipX = false; // No flipping for left-facing directions
        }
        // For 0 and 4, we don't change the flipping
    }
    
    private void UpdateAnimation(int direction)
    {
        // Map 8 directions to 5 animations
        int animIndex = MapDirectionToAnimation(direction) - 1; // Adjust to 0-based index
        
        if (animIndex >= 0 && animIndex < 5)
        {
            _currentAnimationIndex = animIndex;
            
            if (useDirectAnimations)
            {
                // Use direct animation arrays
                if (_isMoving)
                {
                    // Play movement animation
                    if (_animationComponent != null && movementAnimations[_currentAnimationIndex] != null)
                    {
                        _animationComponent.Play(movementAnimations[_currentAnimationIndex].name);
                    }
                }
                else
                {
                    // Display idle sprite
                    if (_spriteRenderer != null && idleSprites[_currentAnimationIndex] != null)
                    {
                        _spriteRenderer.sprite = idleSprites[_currentAnimationIndex];
                    }
                }
            }
            else if (_animator != null)
            {
                // Use Animator Controller
                _animator.SetBool("IsMoving", _isMoving);
                _animator.SetInteger("Direction", _currentAnimationIndex + 1); // Back to 1-based for animator
                _animator.SetBool("FacingRight", _facingRight);
            }
        }
    }
    
    private int MapDirectionToAnimation(int direction)
    {
        // Map 8 directions to your 5 animation states:
        // 1: Up (North)
        // 2: 45° Left-Up (Northwest)
        // 3: Left (West)
        // 4: 45° Left-Down (Southwest)
        // 5: Down (South)
        
        switch (direction)
        {
            case 0: // North
                return 1; // Animation 1: Up
            case 7: // Northwest
                return 2; // Animation 2: 45° Left-Up
            case 6: // West
                return 3; // Animation 3: Left
            case 5: // Southwest
                return 4; // Animation 4: 45° Left-Down
            case 4: // South
                return 5; // Animation 5: Down
            case 1: // Northeast (flipped Northwest)
                return 2; // Same animation as Northwest but flipped
            case 2: // East (flipped West)
                return 3; // Same animation as West but flipped
            case 3: // Southeast (flipped Southwest)
                return 4; // Same animation as Southwest but flipped
            default:
                return 1; // Default to Up
        }
    }
    
    public bool IsMoving()
    {
        return _isMoving;
    }
    
    public void EnableMovement()
    {
        _isMovementEnabled = true;
    }
    
    public void DisableMovement()
    {
        _isMovementEnabled = false;
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
