using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField]
    float spawnInterval;
    float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        
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
            currentTime = spawnInterval;
        }
        else
        {
            currentTime -= 1 * Time.deltaTime;
        }

    }
}
