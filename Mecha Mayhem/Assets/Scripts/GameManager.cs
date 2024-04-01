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
        public float bossDistanceThreshold;
    }

    public static bool arMode;

    public static GameManager instance;

    public static int currentLevel { get; private set; }

    public float distanceAmount { get; private set; }
    float pointsMultiplier;
    float orbPoints;
    float bossPoints;
    float scoreTotal;

    [Header("Play Settings")]
    [SerializeField] PlaySettings settings;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Slider distanceGauge;
    [SerializeField] TMP_Text levelText;
    [SerializeField] Slider bossHealthSlider;
    [SerializeField] TMP_Text bossNameText;

    [Header("Spawner Reference")]
    [SerializeField] LevelSpawner spawner;

    [Header("Scene Reference")]
    [SerializeField] Transform scene;
    public Transform Scene { get => scene; }

    [Header("Player Reference")]
    [SerializeField] PlayerController player;

    [Header("Bosses")]
    [SerializeField] float waitTimeToSpawnBoss = 10f;
    [SerializeField] BossInfo[] bosses;

    Boss currentBoss;

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

        distanceAmount = 0f;
        pointsMultiplier = 1f;

        bossActive = false;
        bossSpawning = false;

        bossHealthSlider.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        scoreText.text = ((int)scoreTotal).ToString();

        if (bossActive == false)
        {
            distanceAmount += pointsMultiplier * Time.fixedDeltaTime;
            if(nextBossID < bosses.Length)
                distanceGauge.value = distanceAmount / bosses[nextBossID].bossDistanceThreshold;

            //When threshold is hit, spawn boss and stop score from increasing
            if (!bossSpawning && nextBossID < bosses.Length && distanceAmount + waitTimeToSpawnBoss >= bosses[nextBossID].bossDistanceThreshold)
            {
                Debug.Log("Spawning");
                StartCoroutine(SpawnBoss());
            }
        } else
        {
            bossHealthSlider.value = currentBoss.CurrentHealth01;
        }

        CalculateScore();
    }

    public void SetPointsMultiplier(float multiplier)
    {
        pointsMultiplier = multiplier;
    }

    public void AddOrb()
    {
        orbPoints++;
    }

    IEnumerator SpawnBoss()
    {
        bossSpawning = true;

        spawner.ToggleObstacleSpawns(false); //Pause obstacle spawns
        spawner.SetBossfight(true); //Tells the spawner its boss fight time
        yield return new WaitForSeconds(waitTimeToSpawnBoss);

        player.ResetHealth();

        currentBoss = bosses[nextBossID].boss.GetComponent<Boss>();

        bossHealthSlider.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);

        bossNameText.text = currentBoss.BossName;
        bossHealthSlider.value = 1;

        bossActive = true;
        bosses[nextBossID].boss.SetActive(true);
    }

    public void BossDefeated()
    {
        bossPoints += currentBoss.Score;

        nextBossID++;
        bossActive = false;
        bossSpawning = false;

        bossHealthSlider.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);

        currentLevel++;
        levelText.text = "Level: " + (currentLevel + 1);

        spawner.ToggleObstacleSpawns(true); //Unpause obstacle spawns
        spawner.SetBossfight(false); //Tells the spawner its not boss fight time
    }

    public GameObject GetCurrentBoss()
    {
        return nextBossID < bosses.Length ? bosses[nextBossID].boss.gameObject : null;
    }

    void CalculateScore()
    {
        scoreTotal = distanceAmount + orbPoints + bossPoints;
    }
}
