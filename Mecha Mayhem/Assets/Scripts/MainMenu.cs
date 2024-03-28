using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Play Settings")]
    [SerializeField] PlaySettings settings;

    bool loadARScene = false;

    public void PlayGame()
    {
        if(loadARScene)
            SceneManager.LoadScene(settings.GetARSceneBuildIndex());
        else
            SceneManager.LoadScene(settings.GetMobileSceneBuildIndex());
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadARScene()
    {
        settings.SetARMode(true);
        loadARScene = true;
    }
    public void LoadMobileScene()
    {
        settings.SetARMode(false);
        loadARScene = false;
    }

    public void SetDifficulty(int difficulty)
    {
        settings.SetDifficulty(difficulty);
    }
}
