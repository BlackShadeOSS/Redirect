using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void mine(GameObject player)
    {
        Debug.Log("Mined"); 
        player.GetComponent<Player>().submitKlepsydra();
        Destroy(gameObject);
    }
    public bool isMined()
    {
        return false;
    }
}
