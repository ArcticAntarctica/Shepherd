using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Start()
    {
        LoadSceneAdditive(Scene.MenuScene.ToString());
    }

    public static void LoadScene(string sceneToLoad, string sceneToUnload)
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }

    public static void LoadSceneAdditive(string sceneToAdd)
    {
        SceneManager.LoadScene(sceneToAdd, LoadSceneMode.Additive);
    }

    public enum Scene
    {
        None = 0,
        MainScene = 1,
        MenuScene = 3,
        Level1 = 4,
    }
}
