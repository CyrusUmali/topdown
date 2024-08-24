using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private List<MobVariants> mobVariants; // List of mob variants with probabilities
    [SerializeField] private int maxMobs; // Maximum number of mobs this spawner can spawn
    [SerializeField] private float spawnInterval = 2f; // Interval between spawns
    [SerializeField] private Collider2D spawnArea; // Area where mobs can be spawned

    private int currentMobCount = 0; // Current number of mobs spawned
    private List<GameObject> spawnedMobs = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnMobs());
    }

    private IEnumerator SpawnMobs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentMobCount < maxMobs)
            {
                Vector2 spawnPosition = GetRandomPositionInArea();
                GameObject mobPrefab = GetRandomMobPrefab();
                if (mobPrefab != null)
                {
                    GameObject mob = Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
                    EnemyHealth mobScript = mob.GetComponent<EnemyHealth>();
                    if (mobScript != null)
                    {
                        mobScript.SetSpawner(this);
                    }
                    spawnedMobs.Add(mob);
                    currentMobCount++;
                }
            }

            CleanUpMobs();
        }
    }

    private Vector2 GetRandomPositionInArea()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private GameObject GetRandomMobPrefab()
    {
        float totalProbability = 0f;
        foreach (var variant in mobVariants)
        {
            totalProbability += variant.spawnProbability;
        }

        float randomPoint = Random.value * totalProbability;

        foreach (var variant in mobVariants)
        {
            if (randomPoint < variant.spawnProbability)
            {
                return variant.mobPrefab;
            }
            else
            {
                randomPoint -= variant.spawnProbability;
            }
        }
        return null;
    }

    // Method to be called by the mob when it is destroyed
    public void NotifyMobDestroyed(GameObject mob)
    {
        spawnedMobs.Remove(mob);
        currentMobCount--;
    }

    private void CleanUpMobs()
    {
        spawnedMobs.RemoveAll(mob => mob == null);
        currentMobCount = spawnedMobs.Count;
    }

    private void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }
    }
}
