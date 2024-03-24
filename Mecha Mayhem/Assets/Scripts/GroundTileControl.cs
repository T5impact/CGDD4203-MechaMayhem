using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundTileControl : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField]
    List<Transform> obstacleSpawns;
    // Assign these in the Inspector with the prefabs you want to spawn
    [SerializeField] GameObject[] prefabsToSpawn;
    //Probability of the tile spawning empty (no obstacles)
    [SerializeField] [Range(0, 1)] float emptyChance = 0.5f;
    [SerializeField] bool spawnObstacles = true;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;
    public Transform origin; //Set in the script that spawns the ground tiles
    [SerializeField] float endPosition = -10;
    [SerializeField] Transform road;
    public Transform Road { get => road; }

    // Start is called before the first frame update
    void Start()
    {
        if (road == null)
            road = transform.GetChild(transform.childCount - 1); //Ensure road is last child in the ground tile prefab

        if (spawnObstacles && Random.Range(0f, 1f) > emptyChance)
            SpawnObstacle();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetSpawnObstacles(bool spawn)
    {
        spawnObstacles = spawn;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position -= transform.forward * moveSpeed * Time.deltaTime;//new Vector3(position.x, position.y, position.z - (moveSpeed * Time.deltaTime));

        if (transform.localPosition.z < endPosition)
        {
            Destroy(this.gameObject);
        }
    }

    public void SpawnObstacle()
    {
        if (transform.childCount >= 3)
        {
            // Select a random prefab index
            int prefabIndex = Random.Range(0, prefabsToSpawn.Length);

            // Get the transform of a randomly selected obstacle spawn
            Transform selectedChild = obstacleSpawns[Random.Range(0, obstacleSpawns.Count)];

            // Check if the prefab at the randomly selected index is not null
            if (prefabsToSpawn[prefabIndex] != null)
            {
                // Instantiate the randomly selected prefab at the position and rotation of the selected child
                Instantiate(prefabsToSpawn[prefabIndex], selectedChild.position, prefabsToSpawn[prefabIndex].transform.rotation, selectedChild);
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
