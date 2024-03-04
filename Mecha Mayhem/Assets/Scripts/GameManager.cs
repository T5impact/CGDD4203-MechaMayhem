using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI scoreText;
    public float scoreAmount;
    public float pointsIncreaser;

    private float scoreThreshold = 30f;

    public GameObject Boss;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        scoreAmount = 0f;
        pointsIncreaser = 1f;
    }

    private void FixedUpdate()
    {
        scoreText.text = ((int)scoreAmount).ToString();
        scoreAmount += pointsIncreaser * Time.fixedDeltaTime;

        if (scoreAmount > scoreThreshold)
        {
            Boss.SetActive(true);
        }
    }
}
