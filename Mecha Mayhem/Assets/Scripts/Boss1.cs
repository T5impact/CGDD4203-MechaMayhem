using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class Boss1 : Boss, IHealth
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float waitTimeBeforeAttack;
        public float smallRingsAttackCooldown;
        public float bigRingAttackCooldown;
    }

    [SerializeField] Boss1Ring bigRing;
    [SerializeField] Boss1Ring[] smallRings;
    [SerializeField] Transform bottomOfShip;
    [SerializeField] Transform[] smallAttackPos;
    [SerializeField] Transform[] bigAttackPos;
    [SerializeField] Transform[] smallEndPos;
    [SerializeField] Transform[] bigEndPos;
    [SerializeField] Boss1Ring[] ringIndex;
    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    AttackSettings currentSettings;

    private int smallIndex;
    private int bigIndex;
    private int index;

    private bool ringMoving;

    Boss1Ring chosenRing;

    // Start is called before the first frame update
    void Awake()
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
            //canAttack = true;

            ResetHealth();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttacking && canAttack)
        {
            chosenRing = ringIndex[index];

            if (chosenRing == ringIndex[0])
            {
                BigAttack();
            }
            else if (chosenRing == ringIndex[1])
            {
                SmallAttack1();
            }
            else if (chosenRing == ringIndex[2])
            {
                SmallAttack2();
            }
            else
            {
                SmallAttack3();
            }

            index = (index + 1) % ringIndex.Length;
        }
    }

    public void SmallAttack1()
    {        
        smallIndex = Random.Range(0, smallAttackPos.Length);
        Transform startingPointSmall = smallAttackPos[smallIndex];

        if (startingPointSmall == smallAttackPos[0])
        {
            Transform endPointSmall = smallEndPos[0];
            StartCoroutine(SmallRingAttackSequence(smallRings[0], startingPointSmall, endPointSmall));
        }
        else if (startingPointSmall == smallAttackPos[1])
        {
            Transform endPointSmall = smallEndPos[1];
            StartCoroutine(SmallRingAttackSequence(smallRings[0], startingPointSmall, endPointSmall));
        }
        else
        {
            Transform endPointSmall = smallEndPos[2];
            StartCoroutine(SmallRingAttackSequence(smallRings[0], startingPointSmall, endPointSmall));
        }
    }

    public void SmallAttack2()
    {
        smallIndex = Random.Range(0, smallAttackPos.Length);
        Transform startingPointSmall = smallAttackPos[smallIndex];

        if (startingPointSmall == smallAttackPos[0])
        {
            Transform endPointSmall = smallEndPos[0];
            StartCoroutine(SmallRingAttackSequence(smallRings[1], startingPointSmall, endPointSmall));
        }
        else if (startingPointSmall == smallAttackPos[1])
        {
            Transform endPointSmall = smallEndPos[1];
            StartCoroutine(SmallRingAttackSequence(smallRings[1], startingPointSmall, endPointSmall));
        }
        else
        {
            Transform endPointSmall = smallEndPos[2];
            StartCoroutine(SmallRingAttackSequence(smallRings[1], startingPointSmall, endPointSmall));
        }
    }

    public void SmallAttack3()
    {
        smallIndex = Random.Range(0, smallAttackPos.Length);
        Transform startingPointSmall = smallAttackPos[smallIndex];

        if (startingPointSmall == smallAttackPos[0])
        {
            Transform endPointSmall = smallEndPos[0];
            StartCoroutine(SmallRingAttackSequence(smallRings[2], startingPointSmall, endPointSmall));
        }
        else if (startingPointSmall == smallAttackPos[1])
        {
            Transform endPointSmall = smallEndPos[1];
            StartCoroutine(SmallRingAttackSequence(smallRings[2], startingPointSmall, endPointSmall));
        }
        else
        {
            Transform endPointSmall = smallEndPos[2];
            StartCoroutine(SmallRingAttackSequence(smallRings[2], startingPointSmall, endPointSmall));
        }
    }

    public void BigAttack()
    {
        bigIndex = Random.Range(0, bigAttackPos.Length);
        Transform startingPointBig = bigAttackPos[bigIndex];

        if (startingPointBig == bigAttackPos[0])
        {
            Transform endPointBig = bigEndPos[0];
            StartCoroutine(BigRingAttackSequence(bigRing, startingPointBig, endPointBig));
        }
        else
        {
            Transform endPointBig = bigEndPos[1];
            StartCoroutine(BigRingAttackSequence(bigRing, startingPointBig, endPointBig));
        }
    }

    IEnumerator SmallRingAttackSequence(Boss1Ring smallRing, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        smallRing.firing = true;

        Quaternion originalRot = bigRing.transform.rotation;

        //Move Ring to bottom of the ship
        StartCoroutine(MoveRing(smallRing, bottomOfShip, smallRing.hoverSpeed, true, false));
        yield return new WaitUntil(() => ringMoving == false);
        //Move Ring to attack position
        StartCoroutine(MoveRing(smallRing, attackPoint, smallRing.hoverSpeed, false, false));
        yield return new WaitUntil(() => ringMoving == false);
        //Match rotation of attack position
        StartCoroutine(MoveRing(smallRing, attackPoint, smallRing.hoverSpeed, true));
        yield return new WaitUntil(() => ringMoving == false);

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);
        smallRing.attacking = true;
        float t = 0;
        Vector3 startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, smallRing.transform.position.y, endPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed / 3;
        }
        smallRing.transform.position = new Vector3(endPoint.position.x, smallRing.transform.position.y, endPoint.position.z);

        //Move Ring back to attack position
        StartCoroutine(MoveRing(smallRing, attackPoint, smallRing.hoverSpeed / 2, false));
        yield return new WaitUntil(() => ringMoving == false);
        smallRing.attacking = false;
        //Rotate to non-attack mode
        StartCoroutine(MoveRing(smallRing, attackPoint, originalRot, smallRing.hoverSpeed, false, bottomOfShip));
        yield return new WaitUntil(() => ringMoving == false);
        //Move back to bottom of the ship
        StartCoroutine(MoveRing(smallRing, bottomOfShip, originalRot, smallRing.hoverSpeed, true));
        yield return new WaitUntil(() => ringMoving == false);

        smallRing.firing = false;
        isAttacking = false;

        StartCoroutine(AttackCooldown(currentSettings.smallRingsAttackCooldown));
    }

    IEnumerator BigRingAttackSequence(Boss1Ring bigRing, Transform attackPoint, Transform endPoint)
    {
        isAttacking = true;
        bigRing.firing = true;

        Quaternion originalRot = bigRing.transform.rotation;

        //Move Ring to bottom of the ship
        StartCoroutine(MoveRing(bigRing, bottomOfShip, bigRing.hoverSpeed, true, false));
        yield return new WaitUntil(() => ringMoving == false);
        //Move Ring to attack position
        StartCoroutine(MoveRing(bigRing, attackPoint, bigRing.hoverSpeed, false, false));
        yield return new WaitUntil(() => ringMoving == false);
        //Match rotation of attack position
        StartCoroutine(MoveRing(bigRing, attackPoint, bigRing.hoverSpeed, true));
        yield return new WaitUntil(() => ringMoving == false);

        yield return new WaitForSeconds(currentSettings.waitTimeBeforeAttack);
        bigRing.attacking = true;
        float t = 0;
        Vector3 startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, bigRing.transform.position.y, endPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed;
        }
        bigRing.transform.position = new Vector3(endPoint.position.x, bigRing.transform.position.y, endPoint.position.z);

        //Move Ring back to attack position
        StartCoroutine(MoveRing(bigRing, attackPoint, bigRing.hoverSpeed, false));
        yield return new WaitUntil(() => ringMoving == false);
        bigRing.attacking = false;
        //Rotate to non-attack mode
        StartCoroutine(MoveRing(bigRing, attackPoint, originalRot, bigRing.hoverSpeed, false, bottomOfShip));
        yield return new WaitUntil(() => ringMoving == false);
        //Move back to bottom of the ship
        StartCoroutine(MoveRing(bigRing, bottomOfShip, originalRot, bigRing.hoverSpeed, true));
        yield return new WaitUntil(() => ringMoving == false);

        bigRing.firing = false;
        isAttacking = false;

        StartCoroutine(AttackCooldown(currentSettings.bigRingAttackCooldown));
    }

    IEnumerator MoveRing(Boss1Ring ring, Transform target, float speed, bool includeY, bool matchRotation = true)
    {
        ringMoving = true;

        float t = 0;
        Quaternion startRot = ring.transform.localRotation;
        Vector3 startPos = ring.transform.position;
        while (t < 1)
        {
            if(matchRotation)
                ring.transform.localRotation = Quaternion.Slerp(startRot, target.localRotation, t);

            if(!includeY)
                ring.transform.position = Vector3.Lerp(startPos, new Vector3(target.position.x, ring.transform.position.y, target.position.z), t);
            else
                ring.transform.position = Vector3.Lerp(startPos, target.position, t);
            yield return null;
            t += Time.deltaTime * speed;
        }
        if(matchRotation)
            ring.transform.rotation = target.localRotation;

        if(!includeY)
            ring.transform.position = new Vector3(target.position.x, ring.transform.position.y, target.position.z);
        else
            ring.transform.position = target.position;

        ringMoving = false;
    }
    IEnumerator MoveRing(Boss1Ring ring, Transform target, Quaternion rotation, float speed, bool includeY)
    {
        ringMoving = true;

        float t = 0;
        Quaternion startRot = ring.transform.localRotation;
        Vector3 startPos = ring.transform.localPosition;
        while (t < 1)
        {
            ring.transform.localRotation = Quaternion.Slerp(startRot, rotation, t);
            if (!includeY)
                ring.transform.localPosition = Vector3.Lerp(startPos, new Vector3(target.localPosition.x, ring.transform.localPosition.y, target.localPosition.z), t);
            else
                ring.transform.localPosition = Vector3.Lerp(startPos, target.localPosition, t);
            yield return null;
            t += Time.deltaTime * speed;
        }
        ring.transform.localRotation = rotation;
        if (!includeY)
            ring.transform.localPosition = new Vector3(target.localPosition.x, ring.transform.localPosition.y, target.localPosition.z);
        else
            ring.transform.localPosition = target.localPosition;

        ringMoving = false;
    }
    IEnumerator MoveRing(Boss1Ring ring, Transform target, Quaternion rotation, float speed, bool includeY, Transform yTransform)
    {
        ringMoving = true;

        float t = 0;
        Quaternion startRot = ring.transform.localRotation;
        Vector3 startPos = ring.transform.localPosition;
        while (t < 1)
        {
            ring.transform.localRotation = Quaternion.Slerp(startRot, rotation, t);
            if (!includeY)
                ring.transform.localPosition = Vector3.Lerp(startPos, new Vector3(target.localPosition.x, yTransform.localPosition.y, target.localPosition.z), t);
            else
                ring.transform.localPosition = Vector3.Lerp(startPos, target.localPosition, t);
            yield return null;
            t += Time.deltaTime * speed;
        }
        ring.transform.localRotation = rotation;
        if (!includeY)
            ring.transform.localPosition = new Vector3(target.localPosition.x, yTransform.localPosition.y, target.localPosition.z);
        else
            ring.transform.localPosition = target.localPosition;

        ringMoving = false;
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

        if(currentHealth <= 0)
        {
            gameManager.BossDefeated();
            gameObject.SetActive(false);
        }
    }
}
