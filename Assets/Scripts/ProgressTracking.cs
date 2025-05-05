using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressTracking : MonoBehaviour
{
    public static SceneLoader.Scene CurrentScene;
    public List<LevelProgress> LevelProgressList;
    [SerializeField] private PersistenceManager _persistenceManager;

    private void Awake()
    {
        LoadGame();
    }

    private void LoadGame()
    {
        _persistenceManager.LoadProgress(LevelProgressList);
    }

    public void SaveGame()
    {
        _persistenceManager.SaveProgress(LevelProgressList);
    }

    public LevelProgress FindCurrentLevel()
    {
        return LevelProgressList.FirstOrDefault(levelProgress => levelProgress.LevelScene == CurrentScene);
    }

    public int FindNextLevelIndex()
    {
        for (var i = 0; i < LevelProgressList.Count; i++)
        {
            if (LevelProgressList[i].LevelScene == CurrentScene && i != LevelProgressList.Count - 1)
                return i + 1;
        }

        return 0;
    }

    [Serializable]
    public class LevelProgress
    {
        public SceneLoader.Scene LevelScene;
        public bool IsLocked;
        public int EarnedStars;
    }
}
