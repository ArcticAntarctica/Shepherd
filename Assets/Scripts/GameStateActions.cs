using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateActions : MonoBehaviour
{
    [SerializeField] private int _maxMoveHistoryCount;
    [SerializeField] private int _historyNavigationIndex;
    private GameScoreActions _gameScoreActions;
    
    [Header("Move History:")]
    [SerializeField] private List<MoveHistory> _takenMoves;
    
    [Header("Starting Layout:")]
    [SerializeField] private GameObject _movableBlocksParent;
    [SerializeField] private List<MovableBlock> _movableBlocks;
    
    [Header("History Control Buttons:")]
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _redoButton;

    private void Start()
    {
        _gameScoreActions = GameObject.Find("GameScoreActions").GetComponent<GameScoreActions>();
        GetStartingPointsPositions();
    }

    private void Update()
    {
        HandleButtonInteractability();
    }

    private void HandleButtonInteractability()
    {
        if (_historyNavigationIndex == 0)
            _undoButton.interactable = false;
        else _undoButton.interactable = true;

        if (_takenMoves.Count == 0 || _historyNavigationIndex == _takenMoves.Count - 1)
            _redoButton.interactable = false;
        else _redoButton.interactable = true;
    }

    public void AddTakenMoveToList(Vector3 takenMove, GameObject movedObject, Vector3 positionBeforeMove)
    {
        DeleteHistoryAfterUndo();
        
        if (!HasObjectBeenMovedFromInitialPosition(movedObject))
            AddInitialPositionToHistory(movedObject, positionBeforeMove);
        
        var newMove = new MoveHistory
        {
            MovedObjectPosition = takenMove,
            MovedObject = movedObject
        };
        
        _takenMoves.Add(newMove);

        DeleteAboveMaxHistory(); 
        
        _historyNavigationIndex = _takenMoves.Count - 1;
    }

    private void AddInitialPositionToHistory(GameObject movedObject, Vector3 positionBeforeMove)
    {
        var movableBlock = FindMovableBlock(movedObject);
        
        var newMove = new MoveHistory
        {
            MovedObjectPosition = positionBeforeMove,
            MovedObject = movedObject,
            IsInitialPosition = true,
        };
        
        _takenMoves.Add(newMove);

        DeleteAboveMaxHistory();
        
        _historyNavigationIndex = _takenMoves.Count - 1;
        
        movableBlock.HasBeenMoved = true;
    }

    private void DeleteHistoryAfterUndo()
    {
        if (_historyNavigationIndex < _takenMoves.Count - 1)
        {
            var isInitialPosition = _takenMoves[_historyNavigationIndex].IsInitialPosition;
            
            for (var i = _historyNavigationIndex; i < _takenMoves.Count; i++)
            {
                if (_takenMoves[_historyNavigationIndex].IsInitialPosition)
                {
                    FindMovableBlock(_takenMoves[_historyNavigationIndex].MovedObject).HasBeenMoved = false;
                }
            }

            if (isInitialPosition)
            {
                _takenMoves.RemoveRange(_historyNavigationIndex, _takenMoves.Count);
            }
            else _takenMoves.RemoveRange(_historyNavigationIndex + 1, (_takenMoves.Count - 1) - _historyNavigationIndex);
        }
    }

    private void DeleteAboveMaxHistory()
    {
        if (_takenMoves.Count > _maxMoveHistoryCount)
        {
            if (_takenMoves[0].IsInitialPosition)
            {
                FindMovableBlock(_takenMoves[0].MovedObject).HasBeenMoved = false;
            }
            
            _takenMoves.RemoveAt(0);
        }
    }

    private void ClearAllHistory()
    {
        _takenMoves.Clear();
        _historyNavigationIndex = 0;

        foreach (var movableBlock in _movableBlocks)
        {
            movableBlock.HasBeenMoved = false;
        }
    }
    
    private void GetStartingPointsPositions()
    {
        for (var i = 0; i < _movableBlocksParent.transform.childCount; i++)
        {
            var startingPoint = new MovableBlock()
            {
                MovableObject = _movableBlocksParent.transform.GetChild(i).gameObject,
                StartingPosition = _movableBlocksParent.transform.GetChild(i).position
            };
            
            _movableBlocks.Add(startingPoint);
        }
    }
    
    public void OnUndoButtonClick()
    {
        if (_historyNavigationIndex != 0)
        {
            var previousMove = _takenMoves[_historyNavigationIndex - 1];
            
            _gameScoreActions.DecrementTakenMovesCounter();

            // Check for void in history
            if (previousMove.MovedObjectPosition == previousMove.MovedObject.transform.position)
            {
                previousMove = _takenMoves[_historyNavigationIndex - 2];
                previousMove.MovedObject.transform.position = previousMove.MovedObjectPosition;

                _historyNavigationIndex -= 2;
                return;
            }
            
            previousMove.MovedObject.transform.position = previousMove.MovedObjectPosition;

            _historyNavigationIndex--;
        }
    }

    public void OnRedoButtonClick()
    {
        if (_historyNavigationIndex < _takenMoves.Count - 1)
        {
            var nextMove = _takenMoves[_historyNavigationIndex + 1];
            
            _gameScoreActions.IncrementTakenMovesCounter();

            // Check for void in history
            if (nextMove.MovedObjectPosition == nextMove.MovedObject.transform.position && _historyNavigationIndex != _takenMoves.Count)
            {
                nextMove = _takenMoves[_historyNavigationIndex + 2];
                nextMove.MovedObject.transform.position = nextMove.MovedObjectPosition;

                _historyNavigationIndex += 2;
                return;
            }
            
            nextMove.MovedObject.transform.position = nextMove.MovedObjectPosition;

            _historyNavigationIndex++;
        }
    }

    public void OnResetButtonClick()
    {
        _gameScoreActions.ResetTakenMovesCounter();
        
        foreach (var movableBlock in _movableBlocks)
        {
            movableBlock.MovableObject.transform.position = movableBlock.StartingPosition;
        }
        
        ClearAllHistory();
    }
    

    private bool HasObjectBeenMovedFromInitialPosition(GameObject movableObject)
    {
        foreach (var movableBlock in _movableBlocks)
        {
            if (movableBlock.MovableObject == movableObject)
            {
                if (movableBlock.HasBeenMoved)
                    return true;
                
                return false;
            }
        }

        return false;
    }

    private MovableBlock FindMovableBlock(GameObject movableObject)
    {
        return _movableBlocks.FirstOrDefault(movableBlock => movableBlock.MovableObject == movableObject);
    }

    [Serializable]
    public class MoveHistory
    {
        public GameObject MovedObject;
        public Vector3 MovedObjectPosition;
        public bool IsInitialPosition;
    }

    [Serializable]
    public class MovableBlock
    {
        public GameObject MovableObject;
        public Vector3 StartingPosition;
        public bool HasBeenMoved;
    }
}