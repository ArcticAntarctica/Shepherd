using UnityEngine;
using UnityEngine.UI;

public class SettingsActions : MonoBehaviour
{
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _sfxVolume;
    private SoundManager _soundManager;
    
    private void Awake()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void OnEnable()
    {
        _musicVolume.value = _soundManager.GetMusicVolume();
        _sfxVolume.value = _soundManager.GetSfxVolume();
    }

    public void OnVolumeSettingsChanged()
    {
        _soundManager.SetVolumeSettings(_musicVolume.value, _sfxVolume.value);
    }

    public void OnCloseButtonClick()
    {
        _soundManager.SaveSettings();
        gameObject.SetActive(false);
    }
}
