using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Ring : MonoBehaviour
{
    [SerializeField] public float hoverSpeed = 2f;
    [SerializeField] public float rotateSpeed = 10f;
    [SerializeField] Vector3 upperHoverPoint;
    [SerializeField] Vector3 lowerHoverPoint;
    [SerializeField] int direction = 1;

    public bool firing;

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
}
