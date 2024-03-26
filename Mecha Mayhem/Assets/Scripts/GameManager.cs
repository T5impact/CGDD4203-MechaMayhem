using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Boss1;
    public TextMeshProUGUI scoreText;
    public GroundTileControl spawner;

    public float scoreAmount;
    public float pointsIncreaser;
    private float scoreThreshold = 5f;
    private bool BossActive;
    private bool isActive;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        scoreAmount = 0f;
        pointsIncreaser = 1f;

        isActive = true;
        BossActive = false;
    }

    private void FixedUpdate()
    {
        while (isActive == true)
        {
            scoreText.text = ((int)scoreAmount).ToString();
            scoreAmount += pointsIncreaser * Time.fixedDeltaTime;

            //When threshold is hit, spawn boss and stop score from increasing
            if (scoreAmount > scoreThreshold)
            {
                Boss1.SetActive(true);
                BossActive = true;
                isActive = false;
            }
            return;
        }

        //pause obstacle spawning when boss enters and wait
    }
}
