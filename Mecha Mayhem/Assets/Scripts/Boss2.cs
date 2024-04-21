using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Boss2 : Boss, IHealth
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float waitTimeBeforeMinionAttack;
        public float waitTimeBeforeLaserAttack;
        public float minionAttackCooldown;
        public float laserAttackCooldown;
        public float shieldRecoveryTime;
        public float minionSpeed;
        public float moveSpeed;
        public float minTimeBtwMoving;
        public float maxTimeBtwMoving;
        public float laserSpeed;
        public float laserActivationTime;
    }

    [SerializeField] Transform[] bossPos;
    [SerializeField] Transform[] minionStartPos;
    [SerializeField] Transform[] minionEndPos;
    [SerializeField] Transform[] laserPos;

    [SerializeField] Transform bossBody;
    [SerializeField] GameObject minionStationary;
    [SerializeField] ParticleSystem stationaryTeleportEffect;
    [SerializeField] Boss2Minion minion;
    [SerializeField] Boss2Shield shield;
    [SerializeField] Boss2Laser laser;
    [SerializeField] GameObject spawnLaser;
    [SerializeField] AudioClip laserCharge;
    [SerializeField] AudioClip laserBeam;
    [SerializeField] AudioClip shieldBreak;
    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    AttackSettings currentSettings;

    private int minionIndex;
    private int bossIndex;
    private int laserIndex;
    private int minionAttackCount;
    private AudioSource sfx;

    bool isMoving;
    bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        sfx = gameObject.GetComponent<AudioSource>();

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
            minionAttackCount = 0;

            ResetHealth();
        }
    }

    Coroutine moveCoroutine;
    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && canAttack)
        {
            if (minionAttackCount < 2)
            {
                MinionAttack();
            }
            else
            {
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);
                LaserAttack();
            }
        } else if (!laser.firing && !isMoving && canMove)
        {
            if (bossIndex == 0)
            {
                bossIndex++;
                moveCoroutine = StartCoroutine(BossMovePosition(bossIndex));
            }
            else if (bossIndex == bossPos.Length - 1)
            {
                bossIndex--;
                moveCoroutine = StartCoroutine(BossMovePosition(bossIndex));
            } else
            {
                bool left = UnityEngine.Random.Range(0, 2) == 0;
                if(left)
                {
                    bossIndex++;
                    moveCoroutine = StartCoroutine(BossMovePosition(bossIndex));
                } else
                {
                    bossIndex--;
                    moveCoroutine = StartCoroutine(BossMovePosition(bossIndex));
                }
            }
        }

        if(!sfx.isPlaying)
        {
            sfx.Play();
        }
    }

    public void MinionAttack()
    {
        bossIndex = UnityEngine.Random.Range(0, minionStartPos.Length);
        minionIndex = UnityEngine.Random.Range(0, minionStartPos.Length);
        Transform MinionPoint = minionStartPos[minionIndex];
        Transform endPoint = minionEndPos[minionIndex];

        StartCoroutine(MinionAttackSequence(minionStationary, minion, MinionPoint, endPoint));
    }

    public void LaserAttack()
    {
        laserIndex = UnityEngine.Random.Range(0, 2);

        if (laserIndex == 1)
        {
            StartCoroutine(LaserAttackSequence(laser, 0, bossPos.Length - 1));
        }
        else
        {
            StartCoroutine(LaserAttackSequence(laser, bossPos.Length - 1, 0));
        }
    }

    IEnumerator BossMovePosition(int nextIndex)
    {
        isMoving = true;
        float t = 0;
        Vector3 startPos = bossBody.localPosition;
        while (t < 1)
        {
            bossBody.localPosition = Vector3.Lerp(startPos, bossPos[nextIndex].localPosition, t);
            yield return null;
            t += Time.deltaTime * currentSettings.moveSpeed / 3;
        }
        bossBody.localPosition = bossPos[nextIndex].localPosition;
        isMoving = false;
        StartCoroutine(BossMoveCooldown(UnityEngine.Random.Range(currentSettings.minTimeBtwMoving, currentSettings.maxTimeBtwMoving)));
    }
    IEnumerator BossMoveCooldown(float length)
    {
        canMove = false;
        yield return new WaitForSeconds(length);
        canMove = true;
    }

    IEnumerator MinionAttackSequence(GameObject minionStationary, Boss2Minion minion, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        minion.firing = true;

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeMinionAttack);

        stationaryTeleportEffect.Play();
        minionStationary.SetActive(false);
        minion.gameObject.SetActive(true);
        minion.teleportEffect.Play();


        float t = 0;
        Vector3 startPos = attackPoint.transform.localPosition;
        while (t < 1)
        {
            minion.transform.localPosition = Vector3.Lerp(startPos, endPoint.localPosition, t);
            yield return null;
            t += Time.deltaTime * currentSettings.minionSpeed / 3;
        }
        minion.transform.localPosition = endPoint.localPosition;

        minionAttackCount++;

        minion.teleportEffect.Play();
        yield return new WaitForSeconds(0.5f);

        minionStationary.SetActive(true);
        stationaryTeleportEffect.Play();

        isAttacking = false;
        minion.firing = false;

        minion.gameObject.SetActive(false);

        StartCoroutine(AttackCooldown(currentSettings.minionAttackCooldown));
    }

    IEnumerator LaserAttackSequence(Boss2Laser laser, int attackIndex, int endIndex)
    {
        isAttacking = true;
        laser.firing = true;
        Vector3 startPos = bossBody.localPosition;

        Transform attackPoint = bossPos[attackIndex];
        Transform endPoint = bossPos[endIndex];
        sfx.Stop();
        sfx.PlayOneShot(laserCharge);
        float t = 0;
        while (t < currentSettings.waitTimeBeforeMinionAttack)
        {
            bossBody.localPosition = Vector3.Lerp(startPos, attackPoint.localPosition, t / currentSettings.waitTimeBeforeLaserAttack);
            yield return null;
            t += Time.deltaTime;
        }
        bossBody.localPosition = attackPoint.localPosition;
        sfx.Stop();
        sfx.PlayOneShot(laserBeam);
        laser.gameObject.SetActive(true);
        yield return new WaitForSeconds(currentSettings.laserActivationTime);

        t = 0;
        while (t < 1)
        {
            bossBody.localPosition = Vector3.Lerp(attackPoint.localPosition, endPoint.localPosition, t);
            yield return null;
            t += Time.deltaTime * currentSettings.laserSpeed / 3;
        }
        bossBody.localPosition = endPoint.localPosition;


        laser.laserAnimator.SetTrigger("EndLaser");

        yield return new WaitForSeconds(0.4f);

        laser.gameObject.SetActive(false);
        isAttacking = false;
        laser.firing = false;

        bossIndex = endIndex;

        minionAttackCount = 0;
        StartCoroutine(AttackCooldown(currentSettings.laserAttackCooldown));
        StartCoroutine(BossMoveCooldown(currentSettings.laserAttackCooldown));
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
            Destroy(gameObject);
        }
    }

    public void ShieldDestroyed()
    {
        sfx.Stop();
        sfx.PlayOneShot(shieldBreak);
        StartCoroutine(RegainShieldTimer());
    }

    IEnumerator RegainShieldTimer()
    {
        yield return new WaitForSeconds(currentSettings.shieldRecoveryTime);
        if(currentHealth > 0)
            shield.RegainShield();
    }

    public override GameObject GetBossGameObject()
    {
        return bossBody.gameObject;
    }
}
