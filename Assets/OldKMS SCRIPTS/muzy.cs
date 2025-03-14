using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioSource muzyka;
    [SerializeField] private AudioSource ending;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void stopMuzyka()
    {
        muzyka.Stop();
        ending.Play();
    }
}
