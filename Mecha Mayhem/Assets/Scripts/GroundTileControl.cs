using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField]
    List<Transform> obstacleSpawns;

    [Header("Movement Settings")]
    [SerializeField]
    float moveSpeed;
    public Transform origin; //Set in the script that spawns the ground tiles
    Vector3 position;


    // Start is called before the first frame update
    void Start()
    {
        position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(position.x, position.y, position.z - (moveSpeed * Time.deltaTime));
        position = this.transform.position;

        if(position.z < -2.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
