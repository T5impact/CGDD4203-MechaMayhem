using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Shield : MonoBehaviour
{
    private float hitCount = 0;

    [System.Serializable]
    public struct ShieldSettings
    {
        public float shieldHealth;
    }

    [SerializeField] ShieldSettings normal_settings;
    [SerializeField] ShieldSettings challenging_settings;
    [SerializeField] Boss2 boss;

    ShieldSettings currentSettings;

    public bool isActive { get; private set; }

    private void Start()
    {
        isActive = true;
    }

    public void ToggleShield(bool toggle)
    {
        this.gameObject.SetActive(toggle);
    }

    public void RegainShield()
    {
        ToggleShield(true);
        isActive = true;
        hitCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("MissileAttack"))
        {
            print("Shield Hit");
            hitCount++;
            if (hitCount > currentSettings.shieldHealth)
            {
                isActive = false;
                ToggleShield(false);
                boss.ShieldDestroyed();
            }
            else
            {
                return;
            }
        }
    }
}
