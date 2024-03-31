using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] GameObject onDestroyEffect;
    [SerializeField] float timeToDestroyEffect = 4f;
    [SerializeField] int damageAmount = 5;

    public void SpawnDestroyEffect()
    {
        if (onDestroyEffect != null)
        {
            GameObject effect = Instantiate(onDestroyEffect, transform.position, transform.rotation, transform.parent);
            Destroy(effect, timeToDestroyEffect);
        }
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }
}
