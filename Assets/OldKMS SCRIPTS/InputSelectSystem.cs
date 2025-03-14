using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[ExecuteAlways]
public class GamepadSelection : MonoBehaviour
{   
    public static List<InputDevice> availableInputDevicesForPlayer1;
    public static List<InputDevice> availableInputDevicesForPlayer2;
    public TMP_Dropdown Player1Dropdown;
    public TMP_Dropdown Player2Dropdown;
    private InputDevice Player1SelectedDevice;
    private InputDevice Player2SelectedDevice;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateInputDeviceList();

        Player1Dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(Player1Dropdown, 1); });
        Player2Dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(Player2Dropdown, 2); });
    }

    private void OnDropdownValueChanged(TMP_Dropdown dropdown, int playerIndex)
    {
        if (dropdown.value == 0) return;

        InputDevice selectedDevice = playerIndex == 1 ? availableInputDevicesForPlayer1[dropdown.value - 1] : availableInputDevicesForPlayer2[dropdown.value - 1];
        SelectInputDevice(selectedDevice, playerIndex);
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        Player1Dropdown.onValueChanged.RemoveAllListeners();
        Player2Dropdown.onValueChanged.RemoveAllListeners();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        UpdateInputDeviceList();
    }

    private void UpdateInputDeviceList()
    {
        var devices = new List<InputDevice>();
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad || device is Keyboard)
            {
                devices.Add(device);
                Debug.Log("Detected device: " + device.description.product + " Type: " + device.GetType().Name);
            }
        }
        availableInputDevicesForPlayer1 = new List<InputDevice>(devices);
        availableInputDevicesForPlayer2 = new List<InputDevice>(devices);
        UpdateDropdownOptions();
    }
    
    private bool checkIfDeviceIsSelected(InputDevice device)
    {
        return Player1SelectedDevice == device || Player2SelectedDevice == device;
    }

    private void UpdateDropdownOptions()
    {
        UpdateDropdownOptionsForPlayer(Player1Dropdown, availableInputDevicesForPlayer1, 1);
        UpdateDropdownOptionsForPlayer(Player2Dropdown, availableInputDevicesForPlayer2, 2);
    }

    public void SelectInputDevice(InputDevice device, int playerIndex)
    {
        if (playerIndex == 1)
        {
            if (Player1SelectedDevice == device)
            {
                Player1SelectedDevice = null;
                if (!checkIfDeviceIsSelected(device)) // Check if the device is not selected by the other player
                {
                    availableInputDevicesForPlayer2.Add(device);
                }
            }
            else
            {
                Player1SelectedDevice = device;
                if (Player2SelectedDevice != device) // Check if the device is not selected by the other player
                {
                    availableInputDevicesForPlayer2.Remove(device);
                }
                PlayerPrefs.SetInt("Player1SelectedIndex", Player1Dropdown.value); // Store selected index
            }
        }
        else if (playerIndex == 2)
        {
            if (Player2SelectedDevice == device)
            {
                Player2SelectedDevice = null;
                if (!checkIfDeviceIsSelected(device)) // Check if the device is not selected by the other player
                {
                    availableInputDevicesForPlayer1.Add(device);
                }
            }
            else
            {
                Player2SelectedDevice = device;
                if (Player1SelectedDevice != device) // Check if the device is not selected by the other player
                {
                    availableInputDevicesForPlayer1.Remove(device);
                }
                PlayerPrefs.SetInt("Player2SelectedIndex", Player2Dropdown.value); // Store selected index
            }
        }
        UpdateDropdownOptions();
    }


    private void UpdateDropdownOptionsForPlayer(TMP_Dropdown dropdown, List<InputDevice> availableDevices, int playerIndex)
    {
        dropdown.options.Clear();
        TMP_Dropdown.OptionData placeholderOption = new TMP_Dropdown.OptionData("Wybierz opcje");
        dropdown.options.Add(placeholderOption);

        foreach (var device in availableDevices)
        {
            if (!checkIfDeviceIsSelected(device))
            {
                TMP_Dropdown.OptionData optionData = device is Keyboard ? new TMP_Dropdown.OptionData("Klawiatura") : new TMP_Dropdown.OptionData(device.displayName);
                dropdown.options.Add(optionData);
            }
        }

        // Retrieve and set the last selected index
        int lastSelectedIndex = PlayerPrefs.GetInt(playerIndex == 1 ? "Player1SelectedIndex" : "Player2SelectedIndex", 0);
        dropdown.value = lastSelectedIndex;
        dropdown.RefreshShownValue();
    }



}
