using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class playerCrystalManager : MonoBehaviour
{
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject mineText;
    [SerializeField] private float minPointerDistance = 2.0f;
    [SerializeField] private float rotationSmoothness = 2.0f;
    [SerializeField] private bool isMining = false;
    [SerializeField] private bool isCloseEnough = false;
    
    
    private GameObject closestCrystal;
    private MultiInputSystem inputSystem;
    private MinerGame minerGame;
    private void Start()
    {
        this.inputSystem = GetComponent<MultiInputSystem>();
        this.minerGame = GetComponent<MinerGame>();
    }

    private void OnInteract(InputValue value)
    {
        Debug.Log("Interact");
        if (this.isCloseEnough)
        {
            this.startMining();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isMining)
        this.closestCrystal = getClosestCrystal();
        if (this.closestCrystal != null)
        {
            // if crystal is close enough, hide pointer
            if (getClosestCrystalDistance() < minPointerDistance)
            {
                // jak blisko krysztal
                pointer.SetActive(false);
                isCloseEnough = true;
                
                // wylaczanie tekstu
                if (isMining)
                {
                    mineText.SetActive(false);
                }
                else
                {
                    mineText.SetActive(true);
                }
                
            }
            else
            {
                // jak krysztal daleko
                pointer.transform.rotation = Quaternion.Slerp(pointer.transform.rotation,
                    Quaternion.Euler(0, 0, getClosestCrystalAngle() - 90.0f), rotationSmoothness * Time.deltaTime);
                pointer.SetActive(true);
                isCloseEnough = false;
                mineText.SetActive(false);
            }
        }
        else
        {
            pointer.SetActive(false);
        }
    }

    private GameObject getClosestCrystal()
    {
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");

        GameObject closestCrystal = null;
        float minDistance = Mathf.Infinity;

        Vector3 playerPosition = transform.position;

        foreach (GameObject crystal in crystals)
        {
            if(crystal.GetComponent<Crystal>().isMined()) continue;
            float distance = Vector3.Distance(playerPosition, crystal.transform.position);
            if (distance < minDistance)
            {
                closestCrystal = crystal;
                minDistance = distance;
            }
        }
        return closestCrystal;
    }
    

    private float getClosestCrystalDistance()
    {
        if (this.closestCrystal == null) return 0;
        return Vector3.Distance(transform.position, this.closestCrystal.transform.position);
    }
    
    private float getClosestCrystalAngle()
    {
        if (this.closestCrystal == null) return 0;
        Vector3 direction = this.closestCrystal.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    
    public void startMining()
    {
        this.inputSystem.disableMovement();
        this.isMining = true;
        minerGame.startGame(this.closestCrystal);
    }
    public void stopMining()
    {
        this.inputSystem.enableMovement();
        this.isMining = false;
    }
    
    public bool isMiningNow()
    {
        return this.isMining;
    }
}
