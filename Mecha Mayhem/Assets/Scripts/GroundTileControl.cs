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
    [SerializeField] 
    List<GameObject> prefabsToSpawn;
    [SerializeField] 
    List<GameObject> pickupsToSpawn;
    [SerializeField] GameObject orb;

    //Probability of the tile spawning empty (no obstacles)
    [SerializeField] [Range(0, 1)] float emptyChance = 0.5f;
    [SerializeField] bool spawnObstacles = true;

    bool spawnPickup = false; //Gets set by the terrain spawner on an interval to allow for rare pickups
    bool spawnOrbs = true; //Gets set by the terrain spawner on an interval to allow for orbs
    bool bossFight = false; //Gets set by the terrain spawner when its boss fight time

    [Header("Movement Settings")]
    public Transform origin; //Set in the script that spawns the ground tiles
    [SerializeField] float endPosition = -10;
    [SerializeField] Transform road;
    public Transform Road { get => road; }

    // Start is called before the first frame update
    void Start()
    {
        if (road == null)
            road = transform.GetChild(transform.childCount - 1); //Ensure road is last child in the ground tile prefab

        if (bossFight)
        {
            if (Random.Range(0f, 1f) > emptyChance)
                SpawnBossPickups();
        }
        else 
        {
            if (spawnObstacles && Random.Range(0f, 1f) > emptyChance)
                SpawnObstacle();
        }
    }

    public void SetSpawnObstacles(bool spawn)
    {
        spawnObstacles = spawn;
    }
    public void SetSpawnPickup(bool pickups)
    {
        spawnPickup = pickups;
    }
    public void SetSpawnOrb(bool orbs)
    {
        spawnOrbs = orbs;
    }
    public void SetBossFight(bool bf)
    {
        bossFight = bf;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position -= transform.forward * LevelSpawner.TileSpeed * Time.deltaTime;//new Vector3(position.x, position.y, position.z - (moveSpeed * Time.deltaTime));

        if (transform.localPosition.z < endPosition)
        {
            Destroy(this.gameObject);
        }
    }
    //Method for spawning obstacles outside of the boss fights
    public void SpawnObstacle()
    {
        if (transform.childCount >= 3)
        {
            int spawnedObstacles = 0;
            bool spawnedOrb = false;

            bool noObstacle = Random.Range(0f, 1f) <= emptyChance;

            int guranteedSpawnIndex = Random.Range(0, 3);

            for (int i = 0; i < 3; i++)
            {
                Transform selectedChild = obstacleSpawns[i];
                int prefabIndex = 0;

                // Select a random prefab index. The range gets + 1 if there is a pickup
                if (spawnPickup) { prefabIndex = Random.Range(0, prefabsToSpawn.Count * 2); }
                else { prefabIndex = Random.Range(0, prefabsToSpawn.Count); }

                if (spawnPickup && (prefabIndex >= prefabsToSpawn.Count || i == 2) && i != guranteedSpawnIndex)
                {
                    int pickupIndex = Random.Range(0, pickupsToSpawn.Count);
                    if (pickupsToSpawn[pickupIndex] != null)
                    {
                        Instantiate(pickupsToSpawn[pickupIndex], selectedChild.position, pickupsToSpawn[pickupIndex].transform.rotation, selectedChild);
                    }
                    else
                    {
                        Debug.LogError("The selected pickup is null. Please assign a pickup in the array.");
                    }
                    spawnPickup = false;
                } else
                { 
                    if(spawnPickup && prefabIndex >= prefabsToSpawn.Count) prefabIndex = Random.Range(0, prefabsToSpawn.Count);

                    bool spawnObstacle = Random.Range(0f, Mathf.Max(2f, 4f - GameManager.currentLevel * 0.25f)) <= 0.75f;

                    if (!noObstacle && spawnedObstacles < Mathf.Min(3, 1 + Mathf.FloorToInt((GameManager.currentLevel + 1) / 2f)) && (spawnObstacle || i == guranteedSpawnIndex))
                    {
                        if (prefabsToSpawn[prefabIndex] != null)
                        {
                            // Instantiate the randomly selected prefab at the position and rotation of the selected child
                            Instantiate(prefabsToSpawn[prefabIndex], selectedChild.position, prefabsToSpawn[prefabIndex].transform.rotation, selectedChild);
                            spawnedObstacles++;
                        }
                        else
                        {
                            Debug.LogError("The selected prefab is null. Please assign a prefab in the array.");
                        }
                    } 
                    else
                    {
                        if(!spawnedOrb && Random.Range(0f, 1f) <= 0.5f)
                        {
                            Instantiate(orb, selectedChild.position + orb.transform.localPosition, orb.transform.rotation, selectedChild);
                            spawnedOrb = true;
                        }
                    }
                }
            }

            /*if (spawnPickup) { prefabIndex = Random.Range(0, prefabsToSpawn.Count + 1); }
            else { prefabIndex = Random.Range(0, prefabsToSpawn.Count); }
            
            // Get the transform of a randomly selected obstacle spawn
            //Transform selectedChild = obstacleSpawns[Random.Range(0, obstacleSpawns.Count)];

            //Check to see if it needs to spawn a random pickup
            if(prefabIndex == prefabsToSpawn.Count)
            {
                *//*int pickupIndex = Random.Range(0, pickupsToSpawn.Count);
                if (pickupsToSpawn[pickupIndex] != null)
                {
                    Instantiate(pickupsToSpawn[pickupIndex], selectedChild.position, pickupsToSpawn[pickupIndex].transform.rotation, selectedChild);
                    GameObject.FindObjectOfType<LevelSpawner>().SendMessage("pickupReset"); //Tells the spawner to reset the pickup timer
                }
                else
                {
                    Debug.LogError("The selected pickup is null. Please assign a pickup in the array.");
                }*//*
            }
            // Check if the prefab at the randomly selected index is not null
            else if (prefabsToSpawn[prefabIndex] != null)
            {
                // Instantiate the randomly selected prefab at the position and rotation of the selected child
                
            }
            else
            {
                Debug.LogError("The selected prefab is null. Please assign a prefab in the array.");
            }*/
        }
        else
        {
            Debug.LogError("Parent object needs at least three children and three prefabs assigned.");
        }
    }
    //Boss fight pickup spawn method
    public void SpawnBossPickups()
    {
        if (transform.childCount >= 3)
        {
            if (spawnPickup)
            {
                spawnPickup = false;

                int pickupIndex = Random.Range(0, pickupsToSpawn.Count);

                // Get the transform of a randomly selected obstacle spawn
                Transform selectedChild = obstacleSpawns[Random.Range(0, obstacleSpawns.Count)];

                if (pickupsToSpawn[pickupIndex] != null)
                {
                    Instantiate(pickupsToSpawn[pickupIndex], selectedChild.position, pickupsToSpawn[pickupIndex].transform.rotation, selectedChild);
                    GameObject.FindObjectOfType<LevelSpawner>().SendMessage("pickupReset"); //Tells the spawner to reset the pickup timer
                }
                else
                {
                    Debug.LogError("The selected prefab is null. Please assign a prefab in the array.");
                }
            }
        }
        else
        {
            Debug.LogError("Parent object needs at least three children and three prefabs assigned.");
        }
    }
}
