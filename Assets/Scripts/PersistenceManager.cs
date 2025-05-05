using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PersistenceManager : MonoBehaviour
{
    // File path to save/load data
    private string _progressFilePath;
    private string _settingsFilePath;

    private void Awake()
    {
        // Define the file path (You might want to customize this path)
        _progressFilePath = Path.Combine(Application.dataPath, "progress.json");
        _settingsFilePath = Path.Combine(Application.dataPath, "settings.json");
    }

    // Save level progress to JSON file
    public void SaveProgress(List<ProgressTracking.LevelProgress> progressList)
    {
        // Convert List<ProgressTracking.LevelProgress> to JSON string
        string json = JsonUtility.ToJson(new ProgressData(progressList));

        // Write JSON string to file
        File.WriteAllText(_progressFilePath, json);
    }
    
    // Load level progress from JSON file and fill the existing list
    public void LoadProgress(List<ProgressTracking.LevelProgress> existingList)
    {
        // Check if the file exists
        if (File.Exists(_progressFilePath))
        {
            // Read JSON string from file
            string json = File.ReadAllText(_progressFilePath);

            // Deserialize JSON string to ProgressData object
            ProgressData progressData = JsonUtility.FromJson<ProgressData>(json);

            // Clear the existing list
            existingList.Clear();

            // Add loaded progress values to the existing list
            existingList.AddRange(progressData.LevelProgressList);
        }
    }
    
    // Save sound settings to JSON file
    public void SaveSettings(float musicVolume, float sfxVolume)
    {
        SoundSettingsData soundSettingsData = new SoundSettingsData(musicVolume, sfxVolume);

        // Convert SoundSettingsData to JSON string
        string json = JsonUtility.ToJson(soundSettingsData);

        // Write JSON string to file
        File.WriteAllText(_settingsFilePath, json);
    }

    // Load sound settings from JSON file
    public void LoadSettings(out float musicVolume, out float sfxVolume)
    {
        musicVolume = 1;
        sfxVolume = 1;
        
        // Check if the file exists
        if (File.Exists(_settingsFilePath))
        {
            // Read JSON string from file
            string json = File.ReadAllText(_settingsFilePath);

            // Deserialize JSON string to SoundSettingsData object
            SoundSettingsData soundSettingsData = JsonUtility.FromJson<SoundSettingsData>(json);

            // Set music volume and sfx volume
            musicVolume = soundSettingsData.MusicVolume;
            sfxVolume = soundSettingsData.SfxVolume;
        }
    }

    [System.Serializable]
    private class ProgressData
    {
        public List<ProgressTracking.LevelProgress> LevelProgressList;

        public ProgressData(List<ProgressTracking.LevelProgress> progressList)
        {
            LevelProgressList = progressList;
        }
    }
    
    [System.Serializable]
    private class SoundSettingsData
    {
        public float MusicVolume;
        public float SfxVolume;

        public SoundSettingsData(float musicVolume, float sfxVolume)
        {
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }
    }
}