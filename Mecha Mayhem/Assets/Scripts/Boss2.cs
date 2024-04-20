using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Boss2 : Boss, IHealth
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float waitTimeBeforeAttack;
        public float MinionAttackCooldown;
        public float LaserAttackCooldown;
    }

    [SerializeField] Transform[] BossPos;
    [SerializeField] Transform[] MinionStartPos;
    [SerializeField] Transform[] MinionEndPos;

    [SerializeField] Boss2Minion Minion;
    [SerializeField] Boss2Shield Shield;
    [SerializeField] float MinionSpeed;
    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    AttackSettings currentSettings;

    private Transform originalPoint;
    private int MinionIndex;
    private int BossIndex;
    private int index;
    private int MinionAttackCount;

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
            index = 0;
            MinionAttackCount = 0;
            originalPoint = BossPos[index];

            ResetHealth();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && canAttack)
        {
            if (MinionAttackCount < 2)
            {
                MinionAttack();
            }
            else
            {
                //LaserAttack();
                MinionAttackCount = 0;
            }
        }
    }

    public void MinionAttack()
    {
        BossIndex = UnityEngine.Random.Range(0, MinionStartPos.Length);
        Transform MinionPoint = MinionStartPos[MinionIndex];
        Transform BossPoint = BossPos[BossIndex];

        if (BossPoint == BossPos[0])
        {
            MinionPoint = MinionStartPos[0];
            Transform endPoint = MinionEndPos[0];
            StartCoroutine(MinionAttackSequence(Minion, MinionPoint, endPoint));
        }
        else if (BossPoint == BossPos[1])
        {
            MinionPoint = MinionStartPos[1];
            Transform endPoint = MinionEndPos[1];
            StartCoroutine(MinionAttackSequence(Minion, MinionPoint, endPoint));
        }
        else
        {
            MinionPoint = MinionStartPos[2];
            Transform endPoint = MinionEndPos[2];
            StartCoroutine(MinionAttackSequence(Minion, MinionPoint, endPoint));
        }
    }

    IEnumerator MinionAttackSequence(Boss2Minion Minion, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        Minion.firing = true;
        
        this.gameObject.transform.localPosition = BossPos[BossIndex].transform.localPosition;
        Vector3 originalPos = Minion.transform.position;
        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);

        float t = 0;
        Vector3 startPos = attackPoint.transform.position;
        while (t < 1)
        {
            Minion.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, endPoint.transform.position.y, endPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * MinionSpeed / 3;
        }
        Minion.transform.position = originalPos;

        isAttacking = false;
        Minion.firing = false;

        StartCoroutine(AttackCooldown(currentSettings.MinionAttackCooldown));
    }

    /*IEnumerator LaserAttackSequence(, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);
    }*/

    IEnumerator AttackCooldown(float length)
    {
        canAttack = false;
        yield return new WaitForSeconds(length);
        canAttack = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            gameManager.BossDefeated();
            gameObject.SetActive(false);
        }
    }
}
