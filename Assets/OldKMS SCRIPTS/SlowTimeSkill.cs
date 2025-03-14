using UnityEngine;
using UnityEngine.InputSystem;

public class SlowTimeSkill : MonoBehaviour
{
    public GameObject slowTimeCirclePrefab; // Assign your circle sprite prefab in the inspector

    // This method can be called from a button press or any other input event
    public void ActivateSlowTime()
    {
        // Instantiate the slow time circle at the player's position
        Instantiate(slowTimeCirclePrefab, transform.position, Quaternion.identity);
    }
}