using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectActions : MonoBehaviour
{
    [SerializeField] private int _currentLevelSelectPage;
    [SerializeField] private int _maxLevelSelectPage;
    [SerializeField] private List<LevelSelect> _levelSelectList;
    [SerializeField] private GameObject _breadcrumbs;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private ProgressTracking _progressTracking;
    private SoundManager _soundManager;

    private void Awake()
    {
        _progressTracking = GameObject.Find("ProgressTracking").GetComponent<ProgressTracking>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        SetupBreadcrumbs();
    }

    private void Update()
    {
        ToggleArrowInteractability();
    }

    private void OnEnable()
    {
        _currentLevelSelectPage = 0;
        UpdateLevelSelectScreen();
    }
    
    private void ToggleArrowInteractability()
    {
        if (_currentLevelSelectPage != _maxLevelSelectPage)
        {
            _rightArrow.interactable = true;
        }
        else _rightArrow.interactable = false;

        if (_currentLevelSelectPage > 0)
        {
            _leftArrow.interactable = true;
        }
        else _leftArrow.interactable = false;
    }

    public void OnLeftArrowButtonClick()
    {
        if (_currentLevelSelectPage > 0)
        {
            _currentLevelSelectPage--;
            UpdateLevelSelectScreen();
            MoveActiveBreadcrumb(true);
        }
    }
    
    public void OnRightArrowButtonClick()
    {
        if (_currentLevelSelectPage != _maxLevelSelectPage)
        {
            _currentLevelSelectPage++;
            UpdateLevelSelectScreen();
            MoveActiveBreadcrumb(false);
        }
    }

    private void UpdateLevelSelectScreen()
    {
        var lastLevelNumber = _levelSelectList.Count * _currentLevelSelectPage + 1;

        foreach (var levelSelect in _levelSelectList)
        {
            if (lastLevelNumber <= _progressTracking.LevelProgressList.Count)
            {
                SetButtonLevelNumbers(lastLevelNumber, levelSelect);
                ToggleButtonInteractability(lastLevelNumber, levelSelect);
                UpdateStars(lastLevelNumber, levelSelect);
                AddButtonListeners(levelSelect, lastLevelNumber);
            }
            else
            {
                levelSelect.LevelButton.interactable = false;
                levelSelect.LevelButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            lastLevelNumber++;
        }
    }

    private void SetButtonLevelNumbers(int lastLevelNumber, LevelSelect levelSelect)
    {
        levelSelect.LevelNumberText.text = lastLevelNumber.ToString();
    }
    
    private void ToggleButtonInteractability(int lastLevelNumber, LevelSelect levelSelect)
    {
        if (_progressTracking.LevelProgressList[lastLevelNumber - 1].IsLocked)
        {
            levelSelect.LevelButton.interactable = false;
            levelSelect.LevelButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            levelSelect.LevelButton.interactable = true;
            levelSelect.LevelButton.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void UpdateStars(int lastLevelNumber, LevelSelect levelSelect)
    {
        var starImages = levelSelect.StarsParent.GetComponentsInChildren<Image>();

        for (var i = 0; i < starImages.Length; i++)
        {
            if (i < _progressTracking.LevelProgressList[lastLevelNumber - 1].EarnedStars)
            {
                var starAlphaColor = starImages[i].color;
                starAlphaColor.a = 255f;
                starImages[i].color = starAlphaColor;
            }
            else
            {
                var starAlphaColor = starImages[i].color;
                starAlphaColor.a = 120f / 255f;
                starImages[i].color = starAlphaColor;
            }
        }
    }

    private void SetupBreadcrumbs()
    {
        _maxLevelSelectPage =
            ((_levelSelectList.Count - (_progressTracking.LevelProgressList.Count % _levelSelectList.Count) +
             _progressTracking.LevelProgressList.Count) / _levelSelectList.Count) - 1;

        if (_progressTracking.LevelProgressList.Count % _levelSelectList.Count == 0)
            _maxLevelSelectPage--;
        
        for (var i = 0; i < _maxLevelSelectPage; i++)
        {
            var breadcrumb = Instantiate(_breadcrumbs.transform.GetChild(0), _breadcrumbs.transform);
            breadcrumb.name = "Disabled";
                
            breadcrumb.GetComponent<Image>().enabled = true;
            var newColor = new Color(251, 237, 212, 255);
            breadcrumb.GetComponent<Image>().color = newColor;
        }
    }
    
    private void MoveActiveBreadcrumb(bool moveBack)
    {
        if (!moveBack)
        {
            foreach (Transform breadcrumb in _breadcrumbs.transform)
            {
                if (Mathf.RoundToInt(breadcrumb.GetComponent<Image>().color.a * 255) == 255)
                {
                    breadcrumb.SetSiblingIndex(breadcrumb.GetSiblingIndex() + 1);
                    return;
                }
            }
        }
        else
        {
            foreach (Transform breadcrumb in _breadcrumbs.transform)
            {
                if (Mathf.RoundToInt(breadcrumb.GetComponent<Image>().color.a * 255) == 255)
                {
                    breadcrumb.SetSiblingIndex(breadcrumb.GetSiblingIndex() - 1);
                    return;
                }
            }
        }
    }

    private void AddButtonListeners(LevelSelect levelSelect, int lastLevelNumber)
    {
        levelSelect.LevelButton.onClick.RemoveAllListeners();
        
        _soundManager.SetButtonSfx(levelSelect.LevelButton);
        levelSelect.LevelButton.onClick.AddListener(() => LoadSceneOnLevelClick(lastLevelNumber));
    }

    private void LoadSceneOnLevelClick(int lastLevelNumber)
    {
        ProgressTracking.CurrentScene = _progressTracking.LevelProgressList[lastLevelNumber - 1].LevelScene;
        SceneLoader.LoadScene(ProgressTracking.CurrentScene.ToString(), SceneLoader.Scene.MenuScene.ToString());
    }

    [Serializable]
    private class LevelSelect
    {
        public Button LevelButton;
        public TextMeshProUGUI LevelNumberText;
        public GameObject StarsParent;
    }
}