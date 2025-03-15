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

    [Header("Sprite References")]
    [SerializeField] private Sprite[] idleSprites = new Sprite[5]; // Array for idle sprites
    [SerializeField] private Sprite[] frameOneSprites = new Sprite[5]; // First animation frame
    [SerializeField] private Sprite[] frameTwoSprites = new Sprite[5]; // Second animation frame
    [SerializeField] private float animationFrameRate = 8f; // Frames per second for animation

    [Header("Action Animation")]
    [SerializeField] private Sprite[] shootSprites = new Sprite[5]; // Shooting sprites for 5 directions
    [SerializeField] private Sprite[] hitSprites = new Sprite[5]; // Hit sprites for 5 directions
    private bool _isPlayingActionAnimation = false;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private SpriteRenderer _spriteRenderer;
    private float _animationTimer = 0f;
    private bool _useFrameOne = true;

    private float lastStepTime = 0;
    private bool _facingRight = false;
    private int _currentDirectionIndex = 0;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle animation frame switching if moving
        if (_isMoving && !_isPlayingActionAnimation)
        {
            _animationTimer += Time.deltaTime;
            if (_animationTimer >= 1f / animationFrameRate)
            {
                _animationTimer = 0f;
                _useFrameOne = !_useFrameOne;
                UpdateSprite();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isMovementEnabled) return;

        _rigidbody.linearVelocity = _movementInput * _moveSpeed;
        int newDirection = GetDirection(_movementInput * new Vector2(-1.0f, 1.0f));

        // Only update animation if direction changed or movement state changed
        if (newDirection != LastDirection || _isMoving != (_movementInput != Vector2.zero))
        {
            LastDirection = newDirection;
            _isMoving = (_movementInput != Vector2.zero);

            // Handle sprite flipping based on direction
            UpdateFacingDirection(LastDirection);

            // Update sprite based on movement and direction
            if (!_isPlayingActionAnimation)
            {
                UpdateSprite();
            }
        }

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
        bool wasMoving = _isMoving;
        _isMoving = movement != Vector2.zero;

        // If movement state changed to moving, reset animation timer
        if (!wasMoving && _isMoving)
        {
            _animationTimer = 0f;
            _useFrameOne = true;
        }

        // If not moving, keep the last direction
        if (!_isMoving) return LastDirection;

        // Calculate angle (in degrees) of movement
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;

        // Fix for 90-degree rotation - subtract 90 degrees from the angle
        angle = (angle - 90 + 360) % 360;

        // Convert angle to 8 directions (0-7)
        // 0 = North, 1 = Northeast, 2 = East, 3 = Southeast,
        // 4 = South, 5 = Southwest, 6 = West, 7 = Northwest
        int direction = Mathf.RoundToInt(angle / 45) % 8;

        return direction;
    }

    private void UpdateFacingDirection(int direction)
    {
        if (_spriteRenderer == null) return;

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

    private void UpdateSprite()
    {
        if (_spriteRenderer == null || _isPlayingActionAnimation) return;

        // Map 8 directions to 5 sprite indices
        int spriteIndex = MapDirectionToSpriteIndex(LastDirection);
        _currentDirectionIndex = spriteIndex;

        // Make sure index is valid
        if (spriteIndex >= 0 && spriteIndex < 5)
        {
            if (!_isMoving)
            {
                // Use idle sprite
                _spriteRenderer.sprite = idleSprites[spriteIndex];
            }
            else
            {
                // Alternate between frame one and frame two
                _spriteRenderer.sprite = _useFrameOne ?
                    frameOneSprites[spriteIndex] :
                    frameTwoSprites[spriteIndex];
            }
        }
    }

    private int MapDirectionToSpriteIndex(int direction)
    {
        // Map 8 directions to your 5 sprite indices (0-4):
        // 0: Up (North)
        // 1: 45° Left-Up (Northwest)
        // 2: Left (West)
        // 3: 45° Left-Down (Southwest)
        // 4: Down (South)

        switch (direction)
        {
            case 0: // North
                return 0;
            case 7: // Northwest
                return 1;
            case 6: // West
                return 2;
            case 5: // Southwest
                return 3;
            case 4: // South
                return 4;
            case 1: // Northeast (flipped Northwest)
                return 1;
            case 2: // East (flipped West)
                return 2;
            case 3: // Southeast (flipped Southwest)
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Plays a temporary action animation (shooting or hitting) for a specific duration
    /// </summary>
    /// <param name="actionType">0 = shoot, 1 = hit</param>
    /// <param name="durationFrames">How many frames to show the animation</param>
    /// <returns>Coroutine</returns>
    public IEnumerator PlayActionAnimation(int actionType, int durationFrames)
    {
        if (_isPlayingActionAnimation) yield break;

        _isPlayingActionAnimation = true;
        bool wasMoving = _isMoving;

        // Store original velocity and temporarily stop movement
        Vector2 originalVelocity = _rigidbody.linearVelocity;
        _rigidbody.linearVelocity = Vector2.zero;

        // Select correct sprite array
        Sprite[] actionSprites = actionType == 0 ? shootSprites : hitSprites;

        // Make sure we have a valid direction index
        int spriteIndex = MapDirectionToSpriteIndex(LastDirection);

        // Apply action sprite with correct flip state
        if (spriteIndex >= 0 && spriteIndex < 5 && actionSprites[spriteIndex] != null)
        {
            _spriteRenderer.sprite = actionSprites[spriteIndex];
        }

        // Wait for the specified number of frames
        for (int i = 0; i < durationFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        // Return to normal animation
        _isPlayingActionAnimation = false;

        // Restore movement if it was moving before
        if (wasMoving && _isMovementEnabled)
        {
            _rigidbody.linearVelocity = originalVelocity;
        }

        // Update sprite back to normal state
        UpdateSprite();
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