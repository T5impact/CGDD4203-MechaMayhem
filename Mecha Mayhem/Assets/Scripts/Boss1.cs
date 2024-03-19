using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class Boss1 : Boss
{
    [SerializeField] Boss1Ring bigRing;
    [SerializeField] Boss1Ring[] smallRings;
    [SerializeField] Transform bottomOfShip;
    [SerializeField] Transform[] smallAttackPos;
    [SerializeField] Transform[] bigAttackPos;
    [SerializeField] Transform[] smallEndPos;
    [SerializeField] Transform[] bigEndPos;
    [SerializeField] Boss1Ring[] ringIndex;

    private int smallIndex;
    private int bigIndex;
    public float speed;
    private int index;
    private bool isAttackiing;

    Boss1Ring chosenRing;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        if (this.gameObject.activeInHierarchy)
        {
            isAttackiing = true;
            index = Random.Range(0, ringIndex.Length);
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
        }
        else
        {
            isAttackiing = false;
            return;
        }
    }

    IEnumerator AttackSequence()
    {
        if (this.gameObject.activeInHierarchy)
        {
            isAttackiing = true;
            index = Random.Range(0, ringIndex.Length);
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
        }
        else
        {
            isAttackiing = false;
            yield return null;
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
        Quaternion originalRot = smallRing.transform.rotation;

        smallRing.firing = true;

        //Move Ring to bottom of the ship
        float t = 0;
        Vector3 startPos = smallRing.transform.position;
        while(t < 1)
        {
            smallRing.transform.position = Vector3.Lerp(startPos, bottomOfShip.position, t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed;
        }

        smallRing.transform.position = bottomOfShip.position;

        //Move Ring to attack position
        t = 0;
        startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.position = Vector3.Lerp(startPos, new Vector3(attackPoint.position.x, smallRing.transform.position.y, attackPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed;
        }
        smallRing.transform.position = new Vector3(attackPoint.position.x, smallRing.transform.position.y, attackPoint.position.z);

        //Match rotation of attack position
        t = 0;
        Quaternion startRot = smallRing.transform.rotation;
        startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.rotation = Quaternion.Slerp(startRot, attackPoint.rotation, t);
            smallRing.transform.position = Vector3.Lerp(startPos, attackPoint.position, t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed;
        }
        smallRing.transform.rotation = attackPoint.rotation;
        smallRing.transform.position = attackPoint.position;


        //Do the attack
        yield return new WaitForSeconds(3f);
        t = 0;
        startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, smallRing.transform.position.y, endPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * speed / 2;
        }
        smallRing.transform.position = new Vector3(endPoint.position.x, smallRing.transform.position.y, endPoint.position.z);


        //Rotate to non-attack mode
        t = 0;
        startRot = smallRing.transform.rotation;
        startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.rotation = Quaternion.Slerp(startRot, originalRot, t);
            smallRing.transform.position = Vector3.Lerp(startPos, new Vector3(attackPoint.position.x, bottomOfShip.position.y, attackPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed;
        }
        smallRing.transform.rotation = originalRot;
        smallRing.transform.position = new Vector3(attackPoint.position.x, bottomOfShip.position.y, attackPoint.position.z);

        //Move back to bottom of the ship
        t = 0;
        startPos = smallRing.transform.position;
        while (t < 1)
        {
            smallRing.transform.position = Vector3.Lerp(startPos, bottomOfShip.position, t);
            yield return null;
            t += Time.deltaTime * smallRing.hoverSpeed;
        }

        smallRing.transform.position = bottomOfShip.position;

        //End

        smallRing.firing = false;
    }

    IEnumerator BigRingAttackSequence(Boss1Ring bigRing, Transform attackPoint, Transform endPoint)
    {
        Quaternion originalRot = bigRing.transform.rotation;

        bigRing.firing = true;

        //Move Ring to bottom of the ship
        float t = 0;
        Vector3 startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.position = Vector3.Lerp(startPos, bottomOfShip.position, t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed / 2;
        }

        bigRing.transform.position = bottomOfShip.position;

        //Move Ring to attack position
        t = 0;
        startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.position = Vector3.Lerp(startPos, new Vector3(attackPoint.position.x, bigRing.transform.position.y, attackPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed / 2;
        }
        bigRing.transform.position = new Vector3(attackPoint.position.x, bigRing.transform.position.y, attackPoint.position.z);

        //Match rotation of attack position
        t = 0;
        Quaternion startRot = bigRing.transform.rotation;
        startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.rotation = Quaternion.Slerp(startRot, attackPoint.rotation, t);
            bigRing.transform.position = Vector3.Lerp(startPos, attackPoint.position, t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed / 2;
        }
        bigRing.transform.rotation = attackPoint.rotation;
        bigRing.transform.position = attackPoint.position;


        //Do the attack
        yield return new WaitForSeconds(3f);
        t = 0;
        startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.position = Vector3.Lerp(startPos, new Vector3(endPoint.position.x, bigRing.transform.position.y, endPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * speed / 2;
        }
        bigRing.transform.position = new Vector3(endPoint.position.x, bigRing.transform.position.y, endPoint.position.z);


        //Rotate to non-attack mode
        t = 0;
        startRot = bigRing.transform.rotation;
        startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.rotation = Quaternion.Slerp(startRot, originalRot, t);
            bigRing.transform.position = Vector3.Lerp(startPos, new Vector3(attackPoint.position.x, bottomOfShip.position.y, attackPoint.position.z), t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed / 2;
        }
        bigRing.transform.rotation = originalRot;
        bigRing.transform.position = new Vector3(attackPoint.position.x, bottomOfShip.position.y, attackPoint.position.z);

        //Move back to bottom of the ship
        t = 0;
        startPos = bigRing.transform.position;
        while (t < 1)
        {
            bigRing.transform.position = Vector3.Lerp(startPos, bottomOfShip.position, t);
            yield return null;
            t += Time.deltaTime * bigRing.hoverSpeed / 2;
        }

        bigRing.transform.position = bottomOfShip.position;

        //End

        bigRing.firing = false;
    }
}
