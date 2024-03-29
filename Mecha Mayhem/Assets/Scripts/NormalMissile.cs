using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMissile : Projectile
{
    [SerializeField] int damageAmount;

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Obstacle"))
        {
            if (other.transform.parent && other.transform.parent.tag.Equals("Obstacle"))
            {
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
        }
        else if (other.tag.Equals("Boss"))
        {
            IHealth bossHealth = other.GetComponent<IHealth>();
            if (bossHealth != null) bossHealth.TakeDamage(damageAmount);

            Destroy(gameObject);
        } 
        else if (other.tag.Equals("Shield"))
        {
            Destroy(gameObject);
        }
    }
}
