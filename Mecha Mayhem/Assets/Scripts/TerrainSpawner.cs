using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval;
    [SerializeField] float tileSpeed = 20;

    [SerializeField] GroundTileControl groundTile;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parent;
    [SerializeField] float unitsPerScale = 10;

    bool disableObstacleSpawns;

    float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        //Calculate spawnInterval based on speed pos = speed * time -> time = pos / speed
        spawnInterval = (unitsPerScale * groundTile.Road.localScale.z) / (tileSpeed + 1f);

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
        SpawnTile();
    }

    void SpawnTile()
    {
        if (currentTime <= 0)
        {
            GameObject tile = Instantiate<GameObject>(tilePrefab, transform.position, parent.rotation, parent);
            tile.transform.position += transform.forward * currentTime * 2 * tileSpeed;

            GroundTileControl tileControl = tile.GetComponent<GroundTileControl>();
            if (tileControl)
            {
                tileControl.SetMoveSpeed(tileSpeed);

                if(disableObstacleSpawns)
                {
                    tileControl.SetSpawnObstacles(false);
                }
            }

            currentTime = spawnInterval + currentTime;
        }
        else
        {
            currentTime -= Time.deltaTime;
        }

    }

    public void ToggleObstacleSpawns(bool toggle)
    {
        disableObstacleSpawns = toggle;
    }
}
