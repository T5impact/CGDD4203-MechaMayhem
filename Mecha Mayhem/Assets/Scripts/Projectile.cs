using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rb;

    [Header("On Hit Settings")]
    [SerializeField] GameObject onHitEffect;
    [SerializeField] float timeToDestroyHitEffect = 3f;

    [Header("Settings")]
    [SerializeField] protected float speed;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    protected virtual void OnDestroy()
    {
        if (onHitEffect != null)
        {
            GameObject effect = Instantiate(onHitEffect, transform.position, transform.rotation);
            Destroy(effect, timeToDestroyHitEffect);
        }
    }
}
