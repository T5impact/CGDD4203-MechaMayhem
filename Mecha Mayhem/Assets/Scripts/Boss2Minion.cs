using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Minion : MonoBehaviour
{
    [System.Serializable]
    public struct AttackSettings
    {
        public int damageAmount;
    }

    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;

    public bool firing;

    AttackSettings currentSettings;

    // Start is called before the first frame update
    void Start()
    {
        currentSettings = normal_settings;

        if (GameManager.difficulty == GameManager.Difficulty.Normal)
            currentSettings = normal_settings;
        else if (GameManager.difficulty == GameManager.Difficulty.Challenging)
            currentSettings = challenging_settings;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firing && other.tag.Equals("Player"))
        {
            print("Minion Hit Player");
            IHealth health = other.GetComponent<IHealth>();
            if (health != null) health.TakeDamage(currentSettings.damageAmount);
        }
    }
}
