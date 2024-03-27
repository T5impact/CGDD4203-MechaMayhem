using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected float maxHealth;

    protected float currentHealth;
    public float CurrentHealth { get => currentHealth; }
}
