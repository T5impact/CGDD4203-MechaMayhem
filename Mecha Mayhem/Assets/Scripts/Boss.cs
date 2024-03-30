using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected float maxHealth;

    PlayerHealth health;

    protected float currentHealth;
    public float CurrentHealth { get => currentHealth; }

    private void OnEnable()
    {
        health.SetPlayerHealth(health.maxHealth);
    }
}
