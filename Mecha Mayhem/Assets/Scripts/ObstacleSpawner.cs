using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // Assign these in the Inspector with the prefabs you want to spawn
    public GameObject[] prefabsToSpawn;

    void Start()
    {
        if (transform.childCount >= 3 && prefabsToSpawn.Length == 3)
        {
            // Select a random child index
            int childIndex = Random.Range(0, transform.childCount);
            // Select a random prefab index
            int prefabIndex = Random.Range(0, prefabsToSpawn.Length);

            // Get the transform of the randomly selected child
            Transform selectedChild = transform.GetChild(childIndex);

            // Check if the prefab at the randomly selected index is not null
            if (prefabsToSpawn[prefabIndex] != null)
            {
                // Instantiate the randomly selected prefab at the position and rotation of the selected child
                Instantiate(prefabsToSpawn[prefabIndex], selectedChild.position, selectedChild.rotation, selectedChild);
            }
            else
            {
                Debug.LogError("The selected prefab is null. Please assign a prefab in the array.");
            }
        }
        else
        {
            Debug.LogError("Parent object needs at least three children and three prefabs assigned.");
        }
    }
}
