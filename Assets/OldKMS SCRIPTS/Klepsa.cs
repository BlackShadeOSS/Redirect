using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Klepsa : MonoBehaviour
{
    private RawImage image;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] private GameObject inputHelper;

    private int min = 0; 
    private int max = 4;
    public int current = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        this.image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        this.image.texture = sprites[current].texture;
        if(current >= max)
        {
            inputHelper.SetActive(true);
        }
        else
        {
            inputHelper.SetActive(false);
        }
    }
    
    public void add()
    {
        if (current < max)
        {
            current++;
        }
    }

    public void ultuj(Player player)
    {
        if (current >= max)
        {
            this.fullEvent(player);
            current = 0;
        }
    }
    

    void fullEvent(Player p)
    {
        Debug.Log("event");
        p.skillujLuja();
        // create a child object of circle
        
    }
    
}
