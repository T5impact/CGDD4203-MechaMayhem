using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] Projectile objectToLaunchPrefab; // Assign the prefab of the object you want to launch in the inspector
    [SerializeField] Transform launchPoint; // The point from which the object will be launched
    [SerializeField] float missileLifetime = 15f;

    [Header("AR Settings")]
    [SerializeField] GameObject fovMissileParent;
    [SerializeField] Transform arScene;

    public void LaunchObject()
    {
        // Instantiate the object at the launch point's position and rotation
        if (GameManager.arMode)
        {
            Destroy(Instantiate(objectToLaunchPrefab.gameObject, launchPoint.position, launchPoint.rotation, arScene), missileLifetime);
        }
        else
        {
            Destroy(Instantiate(objectToLaunchPrefab.gameObject, launchPoint.position, launchPoint.rotation), missileLifetime);
        }
    }
}
