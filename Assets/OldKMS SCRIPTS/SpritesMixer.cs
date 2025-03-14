using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesMixer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private MultiInputSystem _multiInputSystem;
    private int _lastDirection = 0;
    private bool _isMoving = false;
    
    private Animator _animator;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _multiInputSystem = GetComponent<MultiInputSystem>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (_multiInputSystem.IsMoving())
        {
            if (_multiInputSystem.LastDirection == 0)
            {
                _animator.SetFloat("X" , 0);
                _animator.SetFloat("Y" , 1);
            }
            else if (_multiInputSystem.LastDirection == 1)
            {
                _animator.SetFloat("X" , -1);
                _animator.SetFloat("Y" , 0);
            }
            else if (_multiInputSystem.LastDirection == 2)
            {
                _animator.SetFloat("X" , 0);
                _animator.SetFloat("Y" , -1);
            }
            else if (_multiInputSystem.LastDirection == 3)
            {
                _animator.SetFloat("X" , 1);
                _animator.SetFloat("Y" , 0);
            }
        }
        else
        {
            _animator.SetFloat("X" , 0);
            _animator.SetFloat("Y" , 0);
        }
    }
}