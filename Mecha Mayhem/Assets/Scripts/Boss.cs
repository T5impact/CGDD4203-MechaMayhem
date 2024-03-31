using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [System.Serializable]
    public struct BossSettings
    {
        public int maxHealth;
        public int score;
    }

    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected BossSettings normal_BossSettings;
    [SerializeField] protected BossSettings challenging_BossSettings;

    protected BossSettings currentBossSettings;

    PlayerHealth health;

    protected int currentHealth;
    public int CurrentHealth { get => currentHealth; }
}
