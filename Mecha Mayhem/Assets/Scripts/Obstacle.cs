using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] GameObject onDestroyEffect;
    [SerializeField] float timeToDestroyEffect = 4f;

    public void SpawnDestroyEffect()
    {
        if (onDestroyEffect != null)
        {
            GameObject effect = Instantiate(onDestroyEffect, transform.position, transform.rotation);
            Destroy(effect, timeToDestroyEffect);
        }
    }
}
