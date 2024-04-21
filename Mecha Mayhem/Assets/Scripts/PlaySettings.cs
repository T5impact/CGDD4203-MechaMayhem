using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaySettings", menuName = "PlaySettings")]
public class PlaySettings : ScriptableObject
{
    [SerializeField] int arBuildIndex;
    [SerializeField] int mobileBuildIndex;
    [SerializeField] GameManager.Difficulty difficulty;
    [SerializeField] bool arMode;

    int score;

    public int GetARSceneBuildIndex()
    {
        return arBuildIndex;
    }

    public int GetMobileSceneBuildIndex()
    {
        return mobileBuildIndex;
    }

    public GameManager.Difficulty GetDifficulty()
    {
        return difficulty;
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = (GameManager.Difficulty)difficulty;
    }

    public bool IsARMode()
    {
        return arMode;
    }

    public void SetARMode(bool arMode)
    {
        this.arMode = arMode;
    }

    public void SetScore(int amount)
    {
        score = amount;
    }
    public int GetScore()
    {
        return score;
    }
}
