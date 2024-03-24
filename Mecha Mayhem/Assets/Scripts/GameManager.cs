using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool arMode;

    public static GameManager instance;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    [Header("Spawner Reference")]
    [SerializeField] TerrainSpawner spawner;

    [Header("Scene Reference")]
    [SerializeField] Transform scene;
    public Transform Scene { get => scene; }

    [Header("Bosses")]
    public GameObject boss1;
    [SerializeField] float boss1ScoreThreshold = 200;

    private bool bossActive;
    private bool isActive;

    public float scoreAmount { get; private set; }
    public float pointsMultiplier { get; private set; }



    private void Awake()
    {
        instance = this;

        //Set shader gloabl variables
        if (scene != null)
        {
            Shader.SetGlobalVector("_ScenePosition", new Vector4(scene.position.x, scene.position.y, scene.position.z, 0));
            Shader.SetGlobalVector("_SceneForward", new Vector4(scene.forward.x, scene.forward.y, scene.position.z, 0));
        }
        else
        {
            Shader.SetGlobalVector("_ScenePosition", new Vector4(0, 0, 0, 0));
            Shader.SetGlobalVector("_SceneForward", new Vector4(0, 0, 1, 0));
        }
    }

    private void Start()
    {
        scoreAmount = 0f;
        pointsMultiplier = 1f;

        isActive = true;
        bossActive = false;
    }

    private void FixedUpdate()
    {
        while (isActive == true)
        {
            scoreText.text = ((int)scoreAmount).ToString();
            scoreAmount += pointsMultiplier * Time.fixedDeltaTime;

            //When threshold is hit, spawn boss and stop score from increasing
            if (scoreAmount > boss1ScoreThreshold)
            {
                boss1.SetActive(true);
                bossActive = true;
                isActive = false;

                spawner.ToggleObstacleSpawns(false);
            }
            return;
        }

        //pause obstacle spawning when boss enters and wait
    }

    public void SetPointsMultiplier(float multiplier)
    {
        pointsMultiplier = multiplier;
    }
}
