using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidsGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    private List<float> spawnSectors = new List<float>();
    
    private const float DEFAULT_OFFSET = 2f;

    private void Start()
    {
        float sum = prefabs.Select(p => p.GetComponent<Asteroid>().Rarity).Sum();
        float stepSum = 0;
        foreach (GameObject prefab in prefabs)
        {
            stepSum += prefab.GetComponent<Asteroid>().Rarity;
            spawnSectors.Add(stepSum / sum);
        }
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            float time = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(time);
            Spawn();
        }
    }

    private void Spawn()
    {
        float seed = Random.Range(0, 1f);
        for (int i = 0; i < spawnSectors.Count; i++)
        {
            if (seed <= spawnSectors[i])
            {
                GameObject go = Instantiate(prefabs[i]);
                go.transform.position = RandomPointOutside();
                Asteroid asteroid = go.GetComponent<Asteroid>();
                Vector3 target = GameManager.Instance.Player.transform.position;
                target += new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f));
                asteroid.Direction = (target - asteroid.transform.position).normalized;
            }
        }
    }
        
    private Vector2 RandomPointOutside()
    {
        bool xLocked = Random.Range(0, 2) == 1;
        bool flip = Random.Range(0, 2) == 1;
        Vector2 min = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        if (xLocked)
        {
            return new Vector2(flip ? min.x - DEFAULT_OFFSET : max.x + DEFAULT_OFFSET, Random.Range(min.y - DEFAULT_OFFSET, max.y + DEFAULT_OFFSET));
        }
        else
        {
            return new Vector2(Random.Range(min.x - DEFAULT_OFFSET, max.x + DEFAULT_OFFSET), flip ? min.y - DEFAULT_OFFSET : max.y + DEFAULT_OFFSET);
        }
    }
}