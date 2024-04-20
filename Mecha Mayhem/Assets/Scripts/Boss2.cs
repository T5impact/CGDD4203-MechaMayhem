using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Boss, IHealth
{
    public struct AttackSettings
    {
        public float waitTimeBeforeAttack;
    }

    public Transform BossPos1;
    public Transform BossPos2;
    public Transform BossPos3;

    [SerializeField] Boss2Minion Minion;
    [SerializeField] Boss2Shield Shield;
    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    AttackSettings currentSettings;


    // Start is called before the first frame update
    void Start()
    {
        currentSettings = normal_settings;
        currentBossSettings = normal_BossSettings;

        if (GameManager.difficulty == GameManager.Difficulty.Normal)
        {
            currentSettings = normal_settings;
            currentBossSettings = normal_BossSettings;
        }
        else if (GameManager.difficulty == GameManager.Difficulty.Challenging)
        {
            currentSettings = challenging_settings;
            currentBossSettings = challenging_BossSettings;
        }

        isAttacking = false;
        if (this.gameObject.activeInHierarchy == true)
        {
            canAttack = true;

            currentHealth = currentBossSettings.maxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && canAttack)
        {

        }
    }

    IEnumerator MinionAttackSequence(Boss2Minion Minion, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);
    }

    /*IEnumerator LaserAttackSequence(, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);
    }*/

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            gameManager.BossDefeated();
            Destroy(gameObject);
        }
    }
}
