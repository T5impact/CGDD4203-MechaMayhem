using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] PlaySettings settings;
    [SerializeField] TextMeshProUGUI scoreText; 
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = $"Score: {settings.GetScore()}";
    }
}
