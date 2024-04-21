using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Body : MonoBehaviour, IHealth
{
    [SerializeField] Boss2 boss;

    public void TakeDamage(int amount)
    {
        boss.TakeDamage(amount);
    }
}
