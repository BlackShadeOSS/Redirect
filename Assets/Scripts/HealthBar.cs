using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Texture hp0;
    public Texture hp1;
    public Texture hp2;
    public Texture hp3;
    public GameObject player;
    public RawImage healthBar;
    void Update()
    {
        float health = player.GetComponent<Zycie>().getHealth();
        if (health >= 3.0f)
        {
            healthBar.texture = hp3;
        } else if (health >= 2.0f)
        {
            healthBar.texture = hp2;
        }
        else if (health >= 1.0f)
        {
            healthBar.texture = hp1;
        }
        else if (health < 1.0f)
        {
            healthBar.texture = hp0;
        }
    }
}
