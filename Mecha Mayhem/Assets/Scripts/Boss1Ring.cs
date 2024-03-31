using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Ring : MonoBehaviour
{
    [System.Serializable]
    public struct AttackSettings
    {
        public int damageAmount;
        public float hoverSpeed;
        public float rotateSpeed;
    }

    [SerializeField] AttackSettings normal_settings;
    [SerializeField] AttackSettings challenging_settings;
    [HideInInspector] public float hoverSpeed = 2f;
    [HideInInspector] public float rotateSpeed = 10f;
    [SerializeField] Vector3 upperHoverPoint;
    [SerializeField] Vector3 lowerHoverPoint;
    [SerializeField] int direction = 1;

    public bool firing;

    AttackSettings currentSettings;

    private void Start()
    {
        currentSettings = normal_settings;

        if (GameManager.difficulty == GameManager.Difficulty.Normal)
            currentSettings = normal_settings;
        else if (GameManager.difficulty == GameManager.Difficulty.Challenging)
            currentSettings = challenging_settings;

        hoverSpeed = currentSettings.hoverSpeed;
        rotateSpeed = currentSettings.rotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.Self);
        if (!firing)
        {
            if (direction == 1)
            {
                Vector3 dir = upperHoverPoint - transform.localPosition;

                transform.localPosition += dir.normalized * hoverSpeed * Time.deltaTime;

                if(dir.magnitude <= 0.1f)
                {
                    direction = -1;
                }
            } else
            {
                Vector3 dir = lowerHoverPoint - transform.localPosition;

                transform.localPosition += dir.normalized * hoverSpeed * Time.deltaTime;

                if (dir.magnitude <= 0.1f)
                {
                    direction = 1;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(firing && other.tag.Equals("Player"))
        {
            print("Ring Hit Player");
            IHealth health = other.GetComponent<IHealth>();
            if (health != null) health.TakeDamage(currentSettings.damageAmount);
        }
    }
}
