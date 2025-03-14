using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float maxHP = 100.0f;
    [SerializeField] private float currentHP = 100.0f;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletTTL = 2.0f;
    [SerializeField] private float bulletVelo = 10.0f;
    [SerializeField] private float bulletKnockbackFactor = 1.0f;
    [SerializeField] private float gunSmoothness = 15.0f;
    [SerializeField] private bool isGamepad = false;
    [SerializeField] private GameObject klepsydra;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject circle;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    PlayerInput playerInput;
    private Vector2 look;
    private playerCrystalManager pcm;
    private Klepsa klepsa;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    void Start()
    {
        this.playerInput = GetComponent<PlayerInput>();
        this.pcm = GetComponent<playerCrystalManager>();
        this.klepsa = this.klepsydra.GetComponent<Klepsa>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        this.currentHP += 0.01f;
        this.currentHP = Mathf.Clamp(this.currentHP, 0, this.maxHP);
    }

    void Update()
     {
         // gradually change color to red
         this.spriteRenderer.color = Color.Lerp(Color.white, Color.red, 1 - (currentHP / maxHP));
         if (isGamepad)
         {
             if (look != Vector2.zero)
             {
                 Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg);
                 gun.transform.rotation =
                     Quaternion.Lerp(gun.transform.rotation, targetRotation, gunSmoothness * Time.deltaTime);
             }
         }
         else
         {
             // calculate angle beetwen player and mouse
             Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
             Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg);
                gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, targetRotation, gunSmoothness * Time.deltaTime);
         }

         if (pcm.isMiningNow())
         {
             this.gun.SetActive(false);
         }
            else
            {
                this.gun.SetActive(true);
            }
     }

    public void playerDead()
    {
        GetComponent<muzy>().stopMuzyka();
        ui.SetActive(false);
        endScreen.SetActive(true);
        scoreText.text = timer.getTotalTime().ToString("F2");
        Time.timeScale = 0;
    }

    
     void OnLook(InputValue value)
     {
         look = value.Get<Vector2>().normalized;
     }

     void OnFire(InputValue value)
     {
         // create bullet child at gun position
         if (gun.transform.childCount > 0)
         {
             
             // Get the position of the first child of the gun
             Transform factGun = gun.transform.GetChild(0);

             // Create an offset for the bullet position
             // Vector3 bulletOffset = new Vector3(0, 0.5f, 0); // Adjust the y value as needed

             // Create bullet at the position of the first child of the gun plus the offset
             GameObject bullet = Instantiate(bulletPrefab, factGun.position, factGun.rotation);
             shootSound.Play();
             bullet.GetComponent<Rigidbody2D>().velocity = gun.transform.right * bulletVelo;
             bullet.GetComponent<Bullet>().bulletKnockbackFactor = bulletKnockbackFactor;
             bullet.GetComponent<Bullet>().bulletVelo = bulletVelo;
             Destroy(bullet, bulletTTL);
         }
     }
     void setKeyboard()
     {
         this.playerInput.defaultControlScheme = "Keyboard";
     }
     void setGamepad()
     {
         this.playerInput.defaultControlScheme = "Gamepad";
     }
     public bool isGamepadControl()
     {
         return this.isGamepad;
     }
     void OnUlt(InputValue value)
     {
        this.klepsa.ultuj(this);
     }
     void OnUnInteract(InputValue value)
     {
         this.klepsa.add();
     }

     public void submitKlepsydra()
     {
         this.klepsa.add();
         this.timer.addTime();
     }

     public void skillujLuja()
     {
         GameObject newCircle = Instantiate(circle, transform.position, Quaternion.identity);
         newCircle.transform.position = transform.position;
     }
     public void playerHit(float damage)
     {
         Debug.Log("Player hit");
            if (isInvincible) return;
         this.currentHP -= damage;
         if (this.currentHP <= 0)
         {
             Destroy(gameObject);
             this.playerDead();
         }
         invincibilityForSeconds(0.3f);
     }
     public void invincibilityForSeconds(float seconds)
     {
         StartCoroutine(invincibilityCoroutine(seconds));
     }
     
     private IEnumerator invincibilityCoroutine(float seconds)
     {
         this.isInvincible = true;
         yield return new WaitForSeconds(seconds);
         this.isInvincible = false;
     }
     public void addDmg(float dmg)
     {
         this.bulletKnockbackFactor += dmg;
     }
     public void addSpeed(float speed)
     {
         this.bulletVelo += speed;
     }
}   
