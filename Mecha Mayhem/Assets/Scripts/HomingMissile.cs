using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : Projectile
{
    [SerializeField] int damageAmount;
    [SerializeField] [Range(0, 1)] float homingSensitivity = 0.5f;

    GameObject target;

    Vector3 dir;

    private void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        if(manager != null)
        {
            target = manager.GetCurrentBoss();
            if (target == null || !target.activeInHierarchy) target = null;
        }

        dir = Vector3.Lerp(transform.forward, transform.up, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            dir = Vector3.Lerp(dir, (target.transform.position - transform.position).normalized, homingSensitivity * Time.deltaTime);
        } else
        {
            dir = transform.forward;
        }

        rb.velocity = dir * speed;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.forward, dir) * transform.rotation, homingSensitivity * Time.deltaTime * 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Boss"))
        {
            IHealth bossHealth = other.GetComponent<IHealth>();
            if (bossHealth != null) bossHealth.TakeDamage(damageAmount);

            SpawnHitEffect();
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Shield"))
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }
}
