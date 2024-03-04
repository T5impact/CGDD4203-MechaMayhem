using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Boss
{
    [SerializeField] Boss1Ring bigRing;
    [SerializeField] Boss1Ring[] smallRings;
    [SerializeField] Transform bottomOfShip;
    [SerializeField] Transform[] smallAttackPos;
    [SerializeField] Transform[] bigAttackPos;

    [SerializeField] Transform[] smallRingAttackPoints;
    [SerializeField] Transform[] bigRingAttackPoints;

    private int smallIndex;
    private int bigIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void Attack()
    {
        smallIndex = Random.Range(0, smallAttackPos.Length);
        Transform startingPointSmall = smallAttackPos[smallIndex];
        //StartCoroutine(SmallRingAttackSequence(smallRings, startingPointSmall));

        bigIndex = Random.Range(0, bigAttackPos.Length);
        Transform startingPointBig = bigAttackPos[bigIndex];
        StartCoroutine(BigRingAttackSequence(bigRing, startingPointBig));
    }

    IEnumerator SmallRingAttackSequence(Boss1Ring smallRing, Transform attackPoint)
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
        yield return new WaitForSeconds(4f);


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

    IEnumerator BigRingAttackSequence(Boss1Ring bigRing, Transform attackPoint)
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
        yield return new WaitForSeconds(8f);


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
