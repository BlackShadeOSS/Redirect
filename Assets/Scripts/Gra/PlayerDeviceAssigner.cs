using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections.Generic;

public class PlayerDeviceAssigner : MonoBehaviour
{
    [SerializeField] private PlayerInput[] playerInputs;

    void Start()
    {
        if (GlobalInputDeviceManager.SelectedDevices != null)
        {
            for (int i = 0; i < playerInputs.Length && i < GlobalInputDeviceManager.SelectedDevices.Length; i++)
            {
                if (GlobalInputDeviceManager.SelectedDevices[i] != null)
                {
                    // Get the InputUser associated with this PlayerInput
                    InputUser user = playerInputs[i].user;
                    
                    // Clear any existing paired devices
                    user.UnpairDevices();
                    
                    // Pair only the device we want this player to use
                    InputUser.PerformPairingWithDevice(GlobalInputDeviceManager.SelectedDevices[i], user);
                }
            }
        }
    }
    
}