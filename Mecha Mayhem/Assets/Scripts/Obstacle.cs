using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] GameObject onDestroyEffect;
    [SerializeField] float timeToDestroyEffect = 4f;
    [SerializeField] int damageAmount;

    public void SpawnDestroyEffect()
    {
        if (onDestroyEffect != null)
        {
            GameObject effect = Instantiate(onDestroyEffect, transform.position, transform.rotation);
            Destroy(effect, timeToDestroyEffect);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Hit");
            Destroy(this.gameObject);
            IHealth health = other.GetComponent<IHealth>();
            if (health != null) health.TakeDamage(damageAmount);
        }
    }
}
