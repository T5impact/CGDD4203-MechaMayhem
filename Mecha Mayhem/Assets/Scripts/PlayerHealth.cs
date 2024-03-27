using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField] int maxHealth;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /*private void OnTriggerEnter(UnityEngine.Collider other)
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
    }*/

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth < 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
