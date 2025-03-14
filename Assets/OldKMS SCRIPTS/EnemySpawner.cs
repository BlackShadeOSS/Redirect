using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRate = 1.0f;
    [SerializeField] private float spawnRadius = 10.0f;
    
    [SerializeField] private GameObject Enemies;
    
    private float nextSpawnTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = Time.time + spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            spawn();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void spawn()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = 0; // Assuming you want to spawn enemies on the ground level
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, Enemies.transform);
    }
}