using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval;
    [SerializeField] float tileSpeed = 20;

    [SerializeField] GroundTileControl groundTile;
    [SerializeField] float unitsPerScale = 10;

    bool disableObstacleSpawns;

    float currentTime;


    // Start is called before the first frame update
    void Start()
    {
        //Calculate spawnInterval based on speed pos = speed * time -> time = pos / speed
        spawnInterval = (unitsPerScale * groundTile.Road.lossyScale.z) / (tileSpeed + 1f);

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
            GameObject tile = GameObject.Instantiate<GameObject>((GameObject)Resources.Load("GroundTile"), this.transform, false);
            tile.transform.parent = null;
            tile.transform.position += Vector3.forward * currentTime * 2 * tileSpeed;

            GroundTileControl tileControl = tile.GetComponent<GroundTileControl>();
            if (tileControl)
            {
                tileControl.SetMoveSpeed(tileSpeed);
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
        disableObstacleSpawns = !toggle;
    }
}
