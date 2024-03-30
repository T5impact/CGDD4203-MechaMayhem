using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] Projectile[] projectiles; // Assign the prefab of the object you want to launch in the inspector
    [SerializeField] Transform launchPoint; // The point from which the object will be launched
    [SerializeField] float missileLifetime = 15f;

    [Header("AR Settings")]
    [SerializeField] Transform fovMissileParent;
    [SerializeField] Transform arScene;

    private void Start()
    {
        if (projectiles == null || projectiles.Length == 0)
            Debug.LogError("No projectiles have been assigned to " + gameObject.name);

        if (launchPoint == null)
            Debug.LogError("No lauch point has been assigned to " + gameObject.name);
    }

    public void LaunchObject(int index)
    {
        // Instantiate the object at the launch point's position and rotation
        if (GameManager.arMode)
        {
            Destroy(Instantiate(projectiles[index].gameObject, launchPoint.position, launchPoint.rotation, arScene), missileLifetime);

            Transform fovMissile = fovMissileParent.GetChild(index);
            if(fovMissile != null) fovMissile.gameObject.SetActive(false);
        }
        else
        {
            Destroy(Instantiate(projectiles[index].gameObject, launchPoint.position, launchPoint.rotation), missileLifetime);
        }
    }

    //Shows the missile visible in front of the camera in AR mode
    public void ShowFOVMissileAR(int index)
    {
        Transform fovMissile = fovMissileParent.GetChild(index);
        if (fovMissile != null) fovMissile.gameObject.SetActive(true);
    }
}
