using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class ErrorFlasher : MonoBehaviour
{
    public TMP_Text errorText;
    public float flashDuration = 0.5f;
    private bool isFlashing = false;
    private void Start()
    {
        StartFlashingIfNeeded();
    }

    private void Update()
    {
        if (LocalMultiplayerControllerSelection.AvailableInputDevices.Length >= 2)
        {
            StopFlashing();
        }
        else
        {
            StartFlashingIfNeeded();
        }
    }

    private void StartFlashingIfNeeded()
    {
        if (!isFlashing && LocalMultiplayerControllerSelection.AvailableInputDevices.Length < 2)
        {
            isFlashing = true;
            StartCoroutine(FlashErrorText());
        }
    }

    private IEnumerator FlashErrorText()
    {
        while (isFlashing)
        {
            errorText.gameObject.SetActive(!errorText.gameObject.activeSelf); 
            yield return new WaitForSeconds(flashDuration); 
        }
    }

    private void StopFlashing()
    {
        if (isFlashing)
        {
            isFlashing = false;
            errorText.gameObject.SetActive(true);
        }
    }
}
