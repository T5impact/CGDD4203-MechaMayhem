using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Mathematics;
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
    [SerializeField] Transform[] LaserPos;

    [SerializeField] Boss2Minion Minion;
    [SerializeField] Boss2Shield Shield;
    [SerializeField] Boss2Laser Laser;
    [SerializeField] GameObject spawnLaser;
    [SerializeField] float MinionSpeed;
    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    AttackSettings currentSettings;

    private Transform originalPoint;
    private int MinionIndex;
    private int BossIndex;
    private int LaserIndex;
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
                LaserAttack();
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

    public void LaserAttack()
    {
        LaserIndex = UnityEngine.Random.Range(0, LaserPos.Length);
        Transform LaserPoint = LaserPos[LaserIndex];
        Transform BossPoint = BossPos[BossIndex];

        if (LaserPoint == LaserPos[0])
        {
            Transform endPoint = LaserPos[1];
            StartCoroutine(LaserLeftAttackSequence(Laser, LaserPoint, endPoint));
        }
        else
        {
            Transform endPoint = LaserPos[0];
            StartCoroutine(LaserRightAttackSequence(Laser, LaserPoint, endPoint));
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

        MinionAttackCount++;

        StartCoroutine(AttackCooldown(currentSettings.MinionAttackCooldown));
    }

    IEnumerator LaserLeftAttackSequence(Boss2Laser Laser, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        Laser.firing = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);

        this.gameObject.transform.localPosition = BossPos[2].transform.localPosition;
        GameObject LaserPrefab = Instantiate(spawnLaser, attackPoint);

        float t = 0;
        Vector3 startPos = LaserPrefab.transform.position;
        while (t < 1)
        {
            LaserPrefab.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z), t);
            this.gameObject.transform.localPosition = Vector3.Lerp(BossPos[2].localPosition, new Vector3(BossPos[1].localPosition.x, BossPos[1].localPosition.y, BossPos[1].localPosition.z), t);
            yield return null;
            t += Time.deltaTime * MinionSpeed / 3;
        }
        LaserPrefab.transform.position = new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z);
        this.gameObject.transform.localPosition = new Vector3(BossPos[1].localPosition.x, BossPos[1].localPosition.y, BossPos[1].localPosition.z);

        isAttacking = false;
        Laser.firing = false;
        Destroy(LaserPrefab);

        MinionAttackCount = 0;
        StartCoroutine(AttackCooldown(currentSettings.LaserAttackCooldown));
    }

    IEnumerator LaserRightAttackSequence(Boss2Laser Laser, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        Laser.firing = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);

        this.gameObject.transform.localPosition = BossPos[1].transform.localPosition;
        GameObject LaserPrefab = Instantiate(spawnLaser, attackPoint);

        float t = 0;
        Vector3 startPos = LaserPrefab.transform.position;
        while (t < 1)
        {
            LaserPrefab.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z), t);
            this.gameObject.transform.localPosition = Vector3.Lerp(BossPos[1].localPosition, new Vector3(BossPos[2].localPosition.x, BossPos[2].localPosition.y, BossPos[2].localPosition.z), t);
            yield return null;
            t += Time.deltaTime * MinionSpeed / 3;
        }
        LaserPrefab.transform.position = new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z);
        this.gameObject.transform.localPosition = new Vector3(BossPos[2].localPosition.x, BossPos[2].localPosition.y, BossPos[2].localPosition.z);

        isAttacking = false;
        Laser.firing = false;
        Destroy(LaserPrefab);

        MinionAttackCount = 0;
        StartCoroutine(AttackCooldown(currentSettings.LaserAttackCooldown));
    }

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
