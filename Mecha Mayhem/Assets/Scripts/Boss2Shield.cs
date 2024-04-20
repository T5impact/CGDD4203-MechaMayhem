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

    ShieldSettings currentSettings;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("MissileAttack"))
        {
            print("Shield Hit");
            hitCount++;
            if (hitCount > currentSettings.shieldHealth)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                return;
            }
        }
    }
}
