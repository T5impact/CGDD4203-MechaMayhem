using UnityEngine;

public class Launcher : MonoBehaviour
{
    public GameObject objectToLaunchPrefab; // Assign the prefab of the object you want to launch in the inspector
    public float launchForce = 1000f; // Adjust the force as needed
    public Transform launchPoint; // The point from which the object will be launched

    [Header("AR Settings")]
    [SerializeField] GameObject fovMissileParent;

    public void LaunchObject()
    {
        // Instantiate the object at the launch point's position and rotation
        GameObject launchedObject = Instantiate(objectToLaunchPrefab, launchPoint.position, launchPoint.rotation);

        // Get the Rigidbody component of the launched object
        Rigidbody rb = launchedObject.GetComponent<Rigidbody>();

        // Apply a force to the launched object to move it forward
        rb.AddForce(launchPoint.forward * launchForce);
    }
}
