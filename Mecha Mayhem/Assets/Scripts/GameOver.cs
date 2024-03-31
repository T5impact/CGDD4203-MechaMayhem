using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] AudioClip death;
    [SerializeField] PlaySettings settings;
    private void Start()
    {
        music.PlayOneShot(death);
    }
    public void Restart()
    {
        if(settings.IsARMode())
            SceneManager.LoadScene(settings.GetARSceneBuildIndex());
        else
            SceneManager.LoadScene(settings.GetMobileSceneBuildIndex());
    }
    public void QuitGame()
    {
        //Application.Quit();
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
}
