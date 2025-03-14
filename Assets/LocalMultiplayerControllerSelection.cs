using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways]

public class LocalMultiplayerControllerSelection : MonoBehaviour
{
    public static InputDevice[] AvailableInputDevices;
    public GameObject exampleToggle;
    public GameObject[] playerToggleGroups;
    public Button startButton;
    [SerializeField]
    public InputDevice[] SelectedDevices;
    
    public void SelectDevice(int index, int playerNumber)
    {
        if (index >= 0 && index < AvailableInputDevices.Length)
        {
            SelectedDevices[playerNumber-1] = AvailableInputDevices[index];
            Debug.Log("Player " + playerNumber + " selected device: " + AvailableInputDevices[index].displayName);
        }
        else
        {
            Debug.Log("Invalid index: " + index);
        }
    }

    private void Start()
    {
        DeleteAllToggles();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        SelectedDevices = new InputDevice[playerToggleGroups.Length];
        UpdateInputDeviceList(); // Initialize the list on enable
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
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
            if (device is Gamepad || device is Keyboard || device is Joystick)
            {
                devices.Add(device);
                Debug.Log("Detected device: " + device.displayName + " Type: " + device.GetType().Name);
            }
        }
        AvailableInputDevices = devices.ToArray();
        UpdateToggleGroupOptions();
    }

    private void UpdateToggleGroupOptions()
    {
        Debug.Log("Updating toggle group options");
        for (int i = 0; i < playerToggleGroups.Length; i++) 
        {
                Debug.Log("Updating PlayerToggleGroup number: " + i);
                var toggleGroup = playerToggleGroups[i];
                var playerNumber = int.Parse(GetNumbers(toggleGroup.name));
                if (toggleGroup.transform.childCount == 0)
                {
                    Debug.Log("No children found for player: " + playerNumber);
                    Debug.Log("setting up toggle group for player: " + playerNumber);
                    for (int j = 0; j < AvailableInputDevices.Length; j++)
                    {
                        CreateToggleForDevice(AvailableInputDevices[j], playerNumber);
                    }
                }
                else
                {
                    foreach (Transform child in toggleGroup.transform)
                    {
                        Debug.Log("Checking if device: " + child.GetComponentInChildren<Text>().text + " is available for player: " + playerNumber);
                        bool isDeviceAvailable = false;
                        for (int j = 0; j < AvailableInputDevices.Length; j++)
                        {
                            if (child.GetComponentInChildren<Text>().text == AvailableInputDevices[j].displayName)
                            {
                                isDeviceAvailable = true;
                                Debug.Log("Device: " + AvailableInputDevices[j].displayName + " is available for player: " + playerNumber);
                                break;
                            }
                        }

                        if (!isDeviceAvailable)
                        {
                            Debug.Log("Device: " + child.GetComponentInChildren<Text>().text + " is not available for player: " + playerNumber);
                            Destroy(child.gameObject);
                        }
                    }

                    for (int j = 0; j < AvailableInputDevices.Length; j++)
                    {
                        Debug.Log("Checking if device: " + AvailableInputDevices[j].displayName + " is available for player: " + playerNumber);
                        bool isDeviceAvailable = false;
                        for (int k = 0; k < toggleGroup.transform.childCount; k++)
                        {
                            if (toggleGroup.transform.GetChild(k).GetComponentInChildren<Text>().text ==
                                AvailableInputDevices[j].displayName)
                            {
                                Debug.Log("Device: " + AvailableInputDevices[j].displayName + " is already available for player: " + playerNumber);
                                isDeviceAvailable = true;
                                break;
                            }
                        }

                        if (!isDeviceAvailable)
                        {
                            CreateToggleForDevice(AvailableInputDevices[j], playerNumber);
                        }
                    }
                }
        }
    }
    
    private void CreateToggleForDevice(InputDevice inputDevice, int playerNumber)
    {
        Debug.Log("Creating toggle for device: " + inputDevice.displayName + " for player: " + playerNumber);
        var toggleGroup = playerToggleGroups[playerNumber - 1];
        var toggle = Instantiate(exampleToggle, toggleGroup.transform);
        toggle.GetComponent<Toggle>().group = toggleGroup.GetComponent<ToggleGroup>();
        toggle.GetComponentInChildren<Text>().text = inputDevice.displayName;
        var index = System.Array.IndexOf(AvailableInputDevices, inputDevice);
        Debug.Log("Current index: " + index + " Device: " + inputDevice.displayName + " Player: " + playerNumber + " ToggleGroup: " + toggleGroup.name);
        toggle.SetActive(true);
        toggle.transform.localPosition = new Vector2(0, -30 * index);
        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => OnToggleValueChanged(index, playerNumber, value));
    }
    
    private void DeleteAllToggles()
    {
        for (int i = 0; i < playerToggleGroups.Length; i++)
        {
            foreach (Transform child in playerToggleGroups[i].transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void BlockTogglesUpdate(int playerNumber)
    {
        Debug.Log("Blocking toggles for player: " + playerNumber);
        for (int i = 0; i < playerToggleGroups.Length; i++)
        {
            if (playerNumber-1 != i)
            {
                foreach (Transform child in playerToggleGroups[i].transform)
                {
                    if (SelectedDevices[playerNumber - 1] != null)
                    {
                        if (child.GetComponentInChildren<Text>().text == SelectedDevices[playerNumber - 1].displayName)
                        {
                            child.GetComponent<Toggle>().interactable = false;
                        }
                        else
                        {
                            child.GetComponent<Toggle>().interactable = true;
                        }
                    }
                    else
                    {
                        child.GetComponent<Toggle>().interactable = true;
                    }
                }
            }
        }
    }

    private void OnToggleValueChanged(int index, int playerNumber, bool isOn)
    {
        Debug.Log("The value is " + isOn + " for player " + playerNumber + " with device index of " + index);
        if (isOn == true)
        {
            SelectDevice(index, playerNumber);
        } else
        {
            SelectedDevices[playerNumber - 1] = null;
        }
        BlockTogglesUpdate(playerNumber);
        CheckIfAllPlayersSelected();
    }

    private void CheckIfAllPlayersSelected()
    {
        bool allPlayersSelected = true;
        for (int i = 0; i < SelectedDevices.Length; i++)
        {
            if (SelectedDevices[i] == null)
            {
                allPlayersSelected = false;
                break;
            }
        }

        if (allPlayersSelected)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
