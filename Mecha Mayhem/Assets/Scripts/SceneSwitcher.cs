using UnityEngine.SceneManagement;

public static class SceneSwitcher
{
    public static void LoadSceneOnTop(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
    }

    public static void UnLoadSceneOnTop(Scene scene)
    {
        int n = SceneManager.sceneCount;
        if (n > 1)
        {
            SceneManager.UnloadSceneAsync(scene.ToString());
        }
    }
}
