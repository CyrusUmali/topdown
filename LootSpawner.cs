using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab; // The prefab for the loot
    [SerializeField] private int lootCount = 1; // Number of loot items to drop

    public void DropItems(Transform killerTransform)
    {
        if (lootPrefab == null)
        {
            Debug.LogError("Loot prefab is not assigned!");
            return;
        }

        for (int i = 0; i < lootCount; i++)
        {
            // Debug.Log($"Spawning loot item {i + 1} at position {transform.position}.");
            GameObject lootInstance = Instantiate(lootPrefab, transform.position, Quaternion.identity);
            Loot lootScript = lootInstance.GetComponent<Loot>();

            if (lootScript != null)
            {
                lootScript.Initialize(killerTransform); // Set the killer as the target
            }
        }
    }
}
