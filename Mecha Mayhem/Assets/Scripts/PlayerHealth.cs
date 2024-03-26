using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;

    private void Start()
    {
        maxHealth = 10;
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.tag.Equals("SmallRing"))
        {
            currentHealth = currentHealth - 5;
            if (currentHealth < 0)
            {
                SceneManager.LoadScene("Game Over");
            }
        }
        else if (other.gameObject.tag.Equals("BigRing"))
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
