using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScoreActions : MonoBehaviour
{
    private ProgressTracking _progressTracking;
    private SoundManager _soundManager;
    
    [Header("Moves Counter:")]
    [SerializeField] private int _totalTakenMovesCount;
    [SerializeField] private int _starsEarned;
    [SerializeField] private TextMeshProUGUI _counterText;
    [SerializeField] private List<MinimumMove> _minimumMoves;

    [Header("Success Screen:")]
    [SerializeField] private GameObject _successScreen;
    [SerializeField] private GameObject _starsParent;
    
    [Header("Finish Point:")]
    [SerializeField] private Vector3 _finishPoint;
    
    public bool IsMovingTowardsFinishPoint(Vector3 upcomingPosition, bool isBlockHorizontal)
    {
        if (!isBlockHorizontal)
            return (Mathf.Abs(upcomingPosition.z - _finishPoint.z) <= 4 && upcomingPosition.x == _finishPoint.x);
        return (Mathf.Abs(upcomingPosition.x - _finishPoint.x) <= 4 && upcomingPosition.z == _finishPoint.z);
    }

    public void IsFinishPoint(GameObject currentObject, bool isBlockHorizontal)
    {
        if (currentObject.CompareTag("Player"))
        {
            if (!isBlockHorizontal)
            {
                if (currentObject.transform.position.z == _finishPoint.z)
                    HandleFinish();
            }
            else
            {
                /*if (currentObject.transform.position.x == _finishPoint.x)
                    HandleFinish();*/
            }
        }
    }

    private void HandleFinish()
    {
        _successScreen.SetActive(true);
        _soundManager.PlayVictorySound();

        var stars = _starsParent.GetComponentsInChildren<Image>();

        for (var i = 0; i < stars.Length; i++)
        {
            if (i > _starsEarned - 1)
            {
                var starAlphaColor = stars[i].color;
                starAlphaColor.a = 120f / 255f;
                stars[i].color = starAlphaColor;
            }
        }

        PauseScreenActions.FreezeMovement = true;
    }

    public void OnContinueButtonClick()
    {
        _progressTracking.FindCurrentLevel().EarnedStars = _starsEarned;
        _progressTracking.LevelProgressList[_progressTracking.FindNextLevelIndex()].IsLocked = false;
        _progressTracking.SaveGame();
        
        SceneLoader.LoadScene(SceneLoader.Scene.MenuScene.ToString(), ProgressTracking.CurrentScene.ToString());
    }

    private void GetFinishPointPosition()
    {
        _finishPoint = GameObject.Find("EndPoint").transform.position;
    }

    private void Start()
    {
        _progressTracking = GameObject.Find("ProgressTracking").GetComponent<ProgressTracking>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        
        GetFinishPointPosition();
        UpdateTakenMovesCounter();
    }

    private void UpdateTakenMovesCounter()
    {
        var currentMinimumMoves = _minimumMoves[0].MoveCount;

        for (var i = 0; i < _minimumMoves.Count; i++)
        {
            if (_totalTakenMovesCount > _minimumMoves[i].MoveCount)
            {
                UpdateStars((_minimumMoves.Count - 1) - i);

                if (i != _minimumMoves.Count - 1)
                    currentMinimumMoves = _minimumMoves[i + 1].MoveCount;
            }
            else if (i == 0)
            {
                UpdateStars(_minimumMoves.Count);
            }
        }
        
        _counterText.text = "Taken moves:<br>" + _totalTakenMovesCount + " / " + currentMinimumMoves;
    }

    private void UpdateStars(int index)
    {
        _starsEarned = index;
        
        for (var i = 0; i < _minimumMoves.Count; i++)
        {
            if (i < _starsEarned)
            {
                var starAlphaColor = _minimumMoves[i].StarIcon.color;
                starAlphaColor.a = 255f;
                _minimumMoves[i].StarIcon.color = starAlphaColor;
            }
            else
            {
                var starAlphaColor = _minimumMoves[i].StarIcon.color;
                starAlphaColor.a = 120f / 255f;
                _minimumMoves[i].StarIcon.color = starAlphaColor;
            }
        }
    }

    public void IncrementTakenMovesCounter()
    {
        _totalTakenMovesCount++;
        UpdateTakenMovesCounter();
    }

    public void DecrementTakenMovesCounter()
    {
        _totalTakenMovesCount--;
        UpdateTakenMovesCounter();
    }

    public void ResetTakenMovesCounter()
    {
        _totalTakenMovesCount = 0;
        UpdateTakenMovesCounter();
    }
    
    [Serializable]
    private class MinimumMove
    {
        public int MoveCount;
        public Image StarIcon;
    }
}