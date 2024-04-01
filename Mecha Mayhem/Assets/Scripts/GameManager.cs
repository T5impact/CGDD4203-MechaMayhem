using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Challenging
    }

    public static Difficulty difficulty;

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
    float pointsMultiplier;
    float orbPoints;
    float bossPoints;
    float scoreTotal;

    [Header("Play Settings")]
    [SerializeField] PlaySettings settings;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Slider scoreGauge;
    [SerializeField] TMP_Text levelText;

    [Header("Spawner Reference")]
    [SerializeField] LevelSpawner spawner;

    [Header("Scene Reference")]
    [SerializeField] Transform scene;
    public Transform Scene { get => scene; }

    [Header("Player Reference")]
    [SerializeField] PlayerController player;

    [Header("Bosses")]
    [SerializeField] float waitTimeToSpawnBoss;
    [SerializeField] BossInfo[] bosses;

    private int nextBossID;

    private bool bossActive;
    private bool bossSpawning;

    private void Awake()
    {
        if (spawner == null) Debug.LogError("No spawner reference has been assigned on the game manager");
        if (player == null) Debug.LogError("No player references has been assigned on the game manager");
        if (bosses == null || bosses.Length == 0) Debug.LogError("No bosses have been assigned on the game manager");

        arMode = settings.IsARMode();
        difficulty = settings.GetDifficulty();

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

        levelText.text = "Level: " + (currentLevel + 1);

        scoreAmount = 0f;
        pointsMultiplier = 1f;
        scoreGauge.maxValue = bosses[nextBossID].bossScoreThreshold;

        bossActive = false;
        bossSpawning = false;
    }

    private void FixedUpdate()
    {
        orbPoints = PlayerController.orbs;
        if (bossActive == false)
        {
            //scoreText.text = ((int)scoreAmount).ToString();
            scoreAmount += pointsMultiplier * Time.fixedDeltaTime;
            scoreGauge.value = scoreAmount;

            //When threshold is hit, spawn boss and stop score from increasing
            if (!bossSpawning && nextBossID < bosses.Length && scoreAmount >= bosses[nextBossID].bossScoreThreshold)
            {
                Debug.Log("Spawning");
                StartCoroutine(SpawnBoss());
            }
        }
        Score();
    }

    public void SetPointsMultiplier(float multiplier)
    {
        pointsMultiplier = multiplier;
    }

    IEnumerator SpawnBoss()
    {
        bossSpawning = true;

        spawner.ToggleObstacleSpawns(false); //Pause obstacle spawns
        spawner.SetBossfight(true); //Tells the spawner its boss fight time
        yield return new WaitForSeconds(waitTimeToSpawnBoss);

        player.ResetHealth();

        bossActive = true;
        bosses[nextBossID].boss.SetActive(true);
    }

    public void BossDefeated()
    {
        nextBossID++;
        bossActive = false;
        bossSpawning = false;

        currentLevel++;
        levelText.text = "Level: " + (currentLevel + 1);

        spawner.ToggleObstacleSpawns(true); //Unpause obstacle spawns
        spawner.SetBossfight(false); //Tells the spawner its not boss fight time

        bossPoints = 15;
    }

    public GameObject GetCurrentBoss()
    {
        return nextBossID < bosses.Length ? bosses[nextBossID].boss : null;
    }

    void Score()
    {
        scoreTotal = scoreAmount + orbPoints + bossPoints;
        scoreText.text = ((int)scoreTotal).ToString();
    }
}
