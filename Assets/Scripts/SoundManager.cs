using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private PersistenceManager _persistenceManager;
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundEffectsSource;
    
    [Header("Music:")]
    [SerializeField] private AudioClip _meadowAmbienceSound;
    
    [Header("Sound Effects:")]
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _blockPickUpSound;
    [SerializeField] private AudioClip _blockPutDownSound;
    [SerializeField] private AudioClip _victorySound;
    
    private void Awake()
    {
        LoadSettings();
        PlayMeadowAmbienceTrack();
    }

    private void LoadSettings()
    {
        _persistenceManager.LoadSettings(out var musicVolume, out var sfxVolume);
        
        _musicSource.volume = musicVolume;
        _soundEffectsSource.volume = sfxVolume;
    }

    public void SaveSettings()
    {
        _persistenceManager.SaveSettings(_musicSource.volume, _soundEffectsSource.volume);
    }
    
    public void SetVolumeSettings(float musicVolume, float sfxVolume)
    {
        _musicSource.volume = musicVolume;
        _soundEffectsSource.volume = sfxVolume;
    }

    public float GetMusicVolume()
    {
        return _musicSource.volume;
    }
    
    public float GetSfxVolume()
    {
        return _soundEffectsSource.volume;
    }
    
    private void PlayMeadowAmbienceTrack()
    {
        _musicSource.Stop();
        _musicSource.clip = _meadowAmbienceSound;
        _musicSource.Play();
    }

    public void PlayBlockPickUpSound()
    {
        _soundEffectsSource.Stop();
        _soundEffectsSource.clip = _blockPickUpSound;
        _soundEffectsSource.Play();
    }
    
    public void PlayBlockPutDownSound()
    {
        _soundEffectsSource.Stop();
        _soundEffectsSource.clip = _blockPutDownSound;
        _soundEffectsSource.Play();
    }
    
    public void PlayVictorySound()
    {
        _soundEffectsSource.Stop();
        _soundEffectsSource.clip = _victorySound;
        _soundEffectsSource.Play();
    }
    
    private void PlayButtonClickSound()
    {
        _soundEffectsSource.Stop();
        _soundEffectsSource.clip = _buttonClickSound;
        _soundEffectsSource.Play();
    }
    
    public void SetButtonSfx(Button customButton)
    {
        if (customButton != null)
        {
            customButton.onClick.AddListener(PlayButtonClickSound);
            return;
        }
        
        var buttonsInScene = Resources.FindObjectsOfTypeAll<Button>();

        foreach (var button in buttonsInScene)
        {
            button.onClick.AddListener(PlayButtonClickSound);
        }
    }
}
