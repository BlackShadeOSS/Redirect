using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections.Generic;

public class PlayerDeviceAssigner : MonoBehaviour
{
    [SerializeField] private PlayerInput[] playerInputs;

    void Start()
    {
        Debug.Log("Starting PlayerDeviceAssigner");
        if (GlobalInputDeviceManager.SelectedDevices != null)
        {
            Debug.Log($"Found {GlobalInputDeviceManager.SelectedDevices.Length} selected devices");
            
            for (int i = 0; i < playerInputs.Length && i < GlobalInputDeviceManager.SelectedDevices.Length; i++)
            {
                if (GlobalInputDeviceManager.SelectedDevices[i] != null)
                {
                    Debug.Log($"Assigning Player {i+1} device: {GlobalInputDeviceManager.SelectedDevices[i].name} (Type: {GlobalInputDeviceManager.SelectedDevices[i].GetType().Name})");
                    
                    // Get the InputUser associated with this PlayerInput
                    InputUser user = playerInputs[i].user;

                    // Clear any existing paired devices
                    user.UnpairDevices();

                    // Pair only the device we want this player to use
                    InputUser.PerformPairingWithDevice(GlobalInputDeviceManager.SelectedDevices[i], user);
                    
                    // Determine and switch to the appropriate control scheme
                    string scheme = DetermineControlScheme(GlobalInputDeviceManager.SelectedDevices[i]);
                    Debug.Log($"Switching Player {i+1} to control scheme: {scheme}");
                    playerInputs[i].SwitchCurrentControlScheme(scheme, GlobalInputDeviceManager.SelectedDevices[i]);
                }
                else
                {
                    Debug.LogWarning($"No device selected for Player {i+1}");
                }
            }
        }
        else
        {
            Debug.LogError("GlobalInputDeviceManager.SelectedDevices is null!");
        }
    }
    
    private string DetermineControlScheme(InputDevice device)
    {
        // Determine appropriate control scheme based on device type
        if (device is Gamepad)
            return "Gamepad";
        else if (device is Keyboard || device is Mouse)
            return "Keyboard&Mouse";
        else
            return "Default";
    }
}