using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPost : MonoBehaviour
{
    [SerializeField] Transform lightSource;
    [SerializeField][Range(0,1)] float chanceToSpawn;

    Vector3 startPos;

    bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        isActive = Random.Range(0, 1f) < chanceToSpawn;
        gameObject.SetActive(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
