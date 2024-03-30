using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField] public int maxHealth;
    [HideInInspector] public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        GetPlayerHealth();
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

    public int GetPlayerHealth()
    {
        return currentHealth;
    }

    public void SetPlayerHealth(int health)
    {
        currentHealth = health;
    }
}
