using UnityEngine;
using UnityEngine.UI;

public class MenuActions : MonoBehaviour
{
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _levelSelectScreen;
    [SerializeField] private GameObject _settingsScreen;
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _soundManager.SetButtonSfx(null);
    }

    #region  Level Select
    public void OnPlayButtonClick()
    {
        _mainScreen.SetActive(false);
        _levelSelectScreen.SetActive(true);
    }
    
    public void OnLevelSelectExitButtonClick()
    {
        _levelSelectScreen.SetActive(false);
        _mainScreen.SetActive(true);
    }
    #endregion

    #region Settings
    public void OnSettingsButtonClick()
    {
        _mainScreen.SetActive(false);
        _settingsScreen.SetActive(true);
    }
    
    public void OnSettingsExitButtonClick()
    {
        _mainScreen.SetActive(true);
    }
    #endregion
    
    public void OnExitGameButtonClick()
    {
        Application.Quit();
    }
}
