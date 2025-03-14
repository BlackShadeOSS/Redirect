using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamepadCaser : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Sprite gamepadSprite;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Player player;
    // Update is called once per frame
    void Update()
    {
        if (player.isGamepadControl())
        {
            if(spriteRenderer != null)
            spriteRenderer.sprite = gamepadSprite;
            if(rawImage != null)
            rawImage.texture = gamepadSprite.texture;
        }
        else
        {
            if(spriteRenderer != null)
            spriteRenderer.sprite = keyboardSprite;
            if(rawImage != null)
            rawImage.texture = keyboardSprite.texture;
        }
    }
}
