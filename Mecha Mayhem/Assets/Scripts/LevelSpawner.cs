using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public static float TileSpeed;

    [System.Serializable]
    public struct Level
    {
        public GameObject[] groundTiles;
    }

    [Header("Spawn Settings")]
    [SerializeField] Level[] levels;
    [SerializeField] Transform parent;
    [SerializeField] float spawnInterval;
    [SerializeField] float normal_tileSpeed = 20;
    [SerializeField] float challenging_tileSpeed = 30;
    [SerializeField] float normalPickupInterval;
    [SerializeField] float bossPickupInterval;

    [SerializeField] GroundTileControl groundTile;
    [SerializeField] float unitsPerScale = 10;

    bool disableObstacleSpawns;
    bool bossFight = false;

    float currentTime;
    float pickupTime;

    float previousTime;

    // Start is called before the first frame update
    void Start()
    {
        TileSpeed = normal_tileSpeed;

        if (GameManager.difficulty == GameManager.Difficulty.Normal)
            TileSpeed = normal_tileSpeed;
        else if (GameManager.difficulty == GameManager.Difficulty.Challenging)
            TileSpeed = challenging_tileSpeed;

        //Calculate spawnInterval based on speed pos = speed * time -> time = pos / speed
        spawnInterval = (unitsPerScale * groundTile.Road.lossyScale.z) / (TileSpeed);

        //Set pre-existing tiles to correct speed
        /*GroundTileControl[] tiles = GameObject.FindObjectsOfType<GroundTileControl>();
        foreach (GroundTileControl tile in tiles)
        {
            tile.SetMoveSpeed(tileSpeed);
        }*/

        if (levels == null || levels.Length == 0)
            Debug.LogError("No levels have been created on level spawner. Create levels through the inspector and assigning appropriate ground tiles.");
    }

    // Update is called once per frame
    void Update()
    {
        spawnTile();
    }
    void spawnTile()
    {
        if (currentTime <= 0)
        {
            GameObject tileToSpawn = levels[Mathf.Min(levels.Length - 1, GameManager.currentLevel)].groundTiles[0];

            if (tileToSpawn == null)
                Debug.LogError("No ground tiles have been assigned to current level on Level Spawner");

            GameObject newTile = Instantiate<GameObject>(tileToSpawn, transform.position, transform.rotation, parent);
            newTile.transform.localPosition += Vector3.forward * (currentTime - Time.deltaTime) * TileSpeed;

            GroundTileControl tileControl = newTile.GetComponent<GroundTileControl>();
            if (tileControl)
            {
                if (pickupTime <= 0)
                {
                    tileControl.SetSpawnPickup(true);
                }
                tileControl.SetBossFight(bossFight);
            }

            currentTime = previousTime = spawnInterval + (currentTime - Time.deltaTime);
        }
        else
        {
            pickupTime -= Time.deltaTime;
            currentTime -= Time.deltaTime;
        }

    }
    public void ToggleObstacleSpawns(bool toggle)
    {
        disableObstacleSpawns = !toggle;
    }
    public void SetBossfight(bool bf)
    {
        bossFight = bf;
    }
    public void pickupReset() //Used to inform the spawner that a pickup was spawned
    {
        if (bossFight) { pickupTime = bossPickupInterval;  }
        else { pickupTime = normalPickupInterval;  }
    }

    public void NextLevel()
    {
        TileSpeed = normal_tileSpeed + 5 * GameManager.currentLevel;

        if (GameManager.difficulty == GameManager.Difficulty.Challenging)
            TileSpeed = challenging_tileSpeed + 5 * GameManager.currentLevel;

        float previousSpawn = spawnInterval;
        spawnInterval = (unitsPerScale * groundTile.Road.lossyScale.z) / (TileSpeed);
        print(previousSpawn + " : " + spawnInterval + " : " + (previousSpawn));
        currentTime -= (previousSpawn - spawnInterval);// + Time.deltaTime* TileSpeed;
        currentTime = Mathf.Max(0, currentTime);
        print(currentTime);
    }
}
