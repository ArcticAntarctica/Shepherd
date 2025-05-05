using UnityEngine;

public class PauseScreenActions : MonoBehaviour
{
    public static bool FreezeMovement;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _successScreen;
    [SerializeField] private GameObject _settingsScreen;

    private void Start()
    {
        FreezeMovement = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_successScreen.activeSelf) return;
            
            _pauseScreen.SetActive(!_pauseScreen.activeSelf);

            if (_pauseScreen.activeSelf)
            {
                FreezeMovement = true;
            }
            else
            {
                _settingsScreen.SetActive(false);
                FreezeMovement = false;
            }
        }
    }

    public void OnResumeButtonClick()
    {
        _pauseScreen.SetActive(false);
        FreezeMovement = false;
    }

    public void OnSettingsButtonClick()
    {
        _settingsScreen.SetActive(true);
    }

    public void OnExitButtonClick()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.MenuScene.ToString(), ProgressTracking.CurrentScene.ToString());
    }
}
