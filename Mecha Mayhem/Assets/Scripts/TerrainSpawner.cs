using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct Level
    {
        public GameObject[] groundTiles;
    }

    [Header("Spawn Settings")]
    [SerializeField] Level[] levels;
    [SerializeField] Transform parent;
    [SerializeField] float spawnInterval;
    [SerializeField] float tileSpeed = 20;
    [SerializeField] float normalPickupInterval;
    [SerializeField] float bossPickupInterval;

    [SerializeField] GroundTileControl groundTile;
    [SerializeField] float unitsPerScale = 10;

    bool disableObstacleSpawns;
    bool bossFight = false;

    float currentTime;
    float pickupTime;

    // Start is called before the first frame update
    void Start()
    {
        //Calculate spawnInterval based on speed pos = speed * time -> time = pos / speed
        spawnInterval = (unitsPerScale * groundTile.Road.lossyScale.z) / (tileSpeed);

        //Set pre-existing tiles to correct speed
        GroundTileControl[] tiles = GameObject.FindObjectsOfType<GroundTileControl>();
        foreach (GroundTileControl tile in tiles)
        {
            tile.SetMoveSpeed(tileSpeed);
        }
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
            GameObject tileToSpawn = levels[GameManager.currentLevel].groundTiles[0];

            GameObject newTile = Instantiate<GameObject>(tileToSpawn, transform.position, transform.rotation, parent);
            newTile.transform.localPosition += Vector3.forward * (currentTime - Time.deltaTime) * tileSpeed;

            GroundTileControl tileControl = newTile.GetComponent<GroundTileControl>();
            if (tileControl)
            {
                if (pickupTime <= 0)
                {
                    tileControl.SetSpawnPickup(true);
                }
                tileControl.SetBossFight(bossFight);
                tileControl.SetMoveSpeed(tileSpeed);
            }

            currentTime = spawnInterval + (currentTime - Time.deltaTime);
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
}
