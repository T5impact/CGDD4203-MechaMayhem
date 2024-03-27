using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct BossInfo
    {
        public GameObject boss;
        public float bossScoreThreshold;
    }

    public static bool arMode;

    public static GameManager instance;

    public static int currentLevel { get; private set; }

    public float scoreAmount { get; private set; }
    public float pointsMultiplier;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    [Header("Spawner Reference")]
    [SerializeField] TerrainSpawner spawner;

    [Header("Scene Reference")]
    [SerializeField] Transform scene;
    public Transform Scene { get => scene; }

    [Header("Bosses")]
    [SerializeField] float waitTimeToSpawnBoss = 10f;
    [SerializeField] BossInfo[] bosses;

    private int nextBossID;

    private bool bossActive;
    private bool isActive;

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

        nextBossID = 0;
        currentLevel = 0;
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
        while (bossActive == false)
        {
            scoreText.text = ((int)scoreAmount).ToString();
            scoreAmount += pointsMultiplier * Time.fixedDeltaTime;

            //When threshold is hit, spawn boss and stop score from increasing
            if (scoreAmount >= bosses[nextBossID].bossScoreThreshold)
            {
                StartCoroutine(SpawnBoss());
            }
        }
    }
    public void SetPointsMultiplier(float multiplier)
    {
        pointsMultiplier = multiplier;
    }

    IEnumerator SpawnBoss()
    {
        bossActive = true;
        isActive = false;

        spawner.ToggleObstacleSpawns(false); //Pause obstacle spawns

        yield return new WaitForSeconds(waitTimeToSpawnBoss);

        bosses[nextBossID].boss.SetActive(true);
    }

    public void BossDefeated()
    {
        nextBossID++;
        bossActive = false;

        currentLevel++;
    }
}
