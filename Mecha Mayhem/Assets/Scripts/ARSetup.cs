using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ARSetup : MonoBehaviour
{
    public static float localScale = 1f;

    [Header("Scene Reference")]
    [SerializeField] Transform scene;
    public bool sceneInitialized { get; private set; }

    TrackableId currentPlaneID;

    [Header("Managers")]
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARSession session;

    List<ARPlane> planes = new List<ARPlane>();

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [Header("UI")]
    [SerializeField] GameObject scanSurroundingsPopUp;
    [SerializeField] GameObject tapToPlacePopUp;

    // Start is called before the first frame update
    private void Start()
    {
        sceneInitialized = false;

        Time.timeScale = 0;

        if (scanSurroundingsPopUp != null)
            scanSurroundingsPopUp.SetActive(true);

        if (tapToPlacePopUp != null)
            tapToPlacePopUp.SetActive(false);

        scene.gameObject.SetActive(false);

        planes = new List<ARPlane>();

        planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;

        //Recalibrate();
    }

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }
    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneInitialized == false)
        {
            if(Input.touchCount > 0) //Detect a tap on the screen to place the scene if scene has not been initialized
            {
                Touch touch = Input.touches[0];

                if(raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
                {
                    scene.gameObject.SetActive(true);

                    Pose hitPose = hits[0].pose;

                    scene.transform.position = hitPose.position + hitPose.up * 2f;
                    scene.transform.rotation = hitPose.rotation;

                    Shader.SetGlobalVector("_ScenePosition", new Vector4(scene.position.x, scene.position.y, scene.position.z, 0));
                    Shader.SetGlobalVector("_SceneForward", new Vector4(scene.forward.x, scene.forward.y, scene.forward.z, 0));

                    currentPlaneID = hits[0].trackableId;

                    if (tapToPlacePopUp != null)
                        tapToPlacePopUp.SetActive(false);

                    sceneInitialized = true;
                    Time.timeScale = 1;

                    HidePlaneMeshes();

                    planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                }
            }
        }
    }

    //AR event for when ar planes are added or removed
    public void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        //Keep a list of current planes to hide later
        foreach(ARPlane plane in args.added)
        {
            planes.Add(plane);
        }
        foreach (ARPlane plane in args.removed)
        {
            planes.Remove(plane);
        }

        //If AR planes are detected and scene hasnt been initialized, prompt player to align scene
        if (sceneInitialized == false)
        {
            if (planes.Count > 0)
            {
                if (scanSurroundingsPopUp != null)
                    scanSurroundingsPopUp.SetActive(false);

                if (tapToPlacePopUp != null)
                    tapToPlacePopUp.SetActive(true);
            }
        } 
        else //If scene is already initialized, check if the current plane was removed and if so, pause the game and reset the AR session
        {
            bool currentPlaneRemoved = false;

            List<ARPlane> removedPlanes = args.removed;
            foreach(ARPlane plane in removedPlanes)
            {
                if(plane.trackableId == currentPlaneID)
                {
                    currentPlaneRemoved = true;
                }
            }

            if(currentPlaneRemoved)
            {
                Recalibrate();
            }
        }
    }

    //Resets the AR session to generate new ar planes
    public void Recalibrate()
    {
        session.Reset();

        planes.Clear();

        if (scanSurroundingsPopUp != null)
            scanSurroundingsPopUp.SetActive(true);

        if (tapToPlacePopUp != null)
            tapToPlacePopUp.SetActive(false);

        sceneInitialized = false;
        Time.timeScale = 0;

        scene.gameObject.SetActive(false);

        planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
    }

    //Hides the ar plane meshes (purely visual)
    void HidePlaneMeshes()
    {
        foreach(ARPlane plane in planes)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            var meshRenderer = plane.GetComponent<MeshRenderer>();
            var lineRenderer = plane.GetComponent<LineRenderer>();

            visualizer.enabled = false;
            meshRenderer.enabled = false;
            lineRenderer.enabled = false;
        }
    }

    //Shows the ar plane meshes (purely visual)
    void ShowPlaneMeshes()
    {
        foreach (ARPlane plane in planes)
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            var meshRenderer = plane.GetComponent<MeshRenderer>();
            var lineRenderer = plane.GetComponent<LineRenderer>();

            visualizer.enabled = true;
            meshRenderer.enabled = true;
            lineRenderer.enabled = true;
        }
    }
}
