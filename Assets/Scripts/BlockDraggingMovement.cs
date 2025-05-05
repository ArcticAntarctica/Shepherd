using UnityEngine;

public class BlockDraggingMovement : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _mouseOffset;
    [SerializeField] private float _mouseZCoord;
    [SerializeField] private int _gridSize = 2;
    [SerializeField] private float _raycastDistance = 0.6f;
    private GameStateActions _gameStateActions;
    private SoundManager _soundManager;
    private GameScoreActions _gameScoreActions;
    private Vector3 _positionBeforeMove;
    
    
    [Header("Block Type:")]
    [SerializeField] private GameObject _1x2Block;
    [SerializeField] private GameObject _1x3Block;

    private void Start()
    {
        _gameStateActions = GameObject.Find("GameStateActions").GetComponent<GameStateActions>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _gameScoreActions = GameObject.Find("GameScoreActions").GetComponent<GameScoreActions>();
    }

    private bool IsBlockHorizontal()
    {
        if (gameObject.transform.rotation.eulerAngles.y == 90)
        {
            return true;
        }

        return false;
    }

    private void OnMouseDown()
    {
        _positionBeforeMove = gameObject.transform.position;
        
        var blockPosition = gameObject.transform.position;
        _mouseZCoord = _camera.WorldToScreenPoint(blockPosition).z;
        _mouseOffset = blockPosition - GetMouseWorldPosition();
        
        if (!PauseScreenActions.FreezeMovement)
            _soundManager.PlayBlockPickUpSound();
    }

    private void OnMouseUp()
    {
        // Increment taken moves if object is in another place than before
        if (_positionBeforeMove != gameObject.transform.position)
        {
            _gameStateActions.AddTakenMoveToList(gameObject.transform.position, gameObject, _positionBeforeMove);
            _gameScoreActions.IncrementTakenMovesCounter();
        }
        
        if (!PauseScreenActions.FreezeMovement)
            _soundManager.PlayBlockPutDownSound();
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePoint = Input.mousePosition;
        mousePoint.z = _mouseZCoord;
        return _camera.ScreenToWorldPoint(mousePoint);
    }
    
    private void OnMouseDrag()
    {
        if (PauseScreenActions.FreezeMovement) return;
        
        // Project the mouse position onto the plane of the block
        // Project the mouse position onto the plane of the block
        var plane = new Plane(Vector3.up, transform.position);
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        
        if (plane.Raycast(ray, out float distance))
        {
            var point = ray.GetPoint(distance);
            var upcomingPosition = point + _mouseOffset;
            
            var upcomingPositionBottomOffset = upcomingPosition;

            // Calculate bottom offset for raycasting
            if (!IsBlockHorizontal())
            {
                if (_1x2Block != null)
                    upcomingPositionBottomOffset.z -= 1;
                else if (_1x3Block != null)
                    upcomingPositionBottomOffset.z -= 2;
            }
            else
            {
                if (_1x2Block != null)
                    upcomingPositionBottomOffset.x -= 1;
                else if (_1x3Block != null)
                    upcomingPositionBottomOffset.x -= 2;
            }
        
            // Apply movement restriction
            if (!IsBlockHorizontal())
            {
                if (_positionBeforeMove.x != Mathf.Round(upcomingPosition.x)) return;
                upcomingPosition.x = _positionBeforeMove.x;
            }
            else
            {
                if (_positionBeforeMove.z != Mathf.Round(upcomingPosition.z)) return;
                upcomingPosition.z = _positionBeforeMove.z;
            }
        
            // Rounds the movement from multiple digits like 1.4532 to just 1 (core of grid movement and snapping)
            upcomingPosition = new Vector3(Mathf.Round(upcomingPosition.x), 1, Mathf.Round(upcomingPosition.z));
            upcomingPositionBottomOffset = new Vector3(Mathf.Round(upcomingPositionBottomOffset.x), 1, Mathf.Round(upcomingPositionBottomOffset.z));

            // Look for raycast collisions
            if (RaycastRectangleDetection(upcomingPosition, upcomingPositionBottomOffset)) return;

        
            // Allow only 1 block movement at a time
            var xAxisDif = Mathf.Abs(transform.position.x - upcomingPosition.x);
            var zAxisDif = Mathf.Abs(transform.position.z - upcomingPosition.z);
            if ((xAxisDif == 1 && zAxisDif == 0) || (xAxisDif == 0 && zAxisDif == 1))
            {
                gameObject.transform.position = upcomingPosition;
            
                _gameScoreActions.IsFinishPoint(gameObject, IsBlockHorizontal());
            }
        }
    }
    
    private bool RaycastRectangleDetection(Vector3 upcomingPosition, Vector3 upcomingPositionBottomOffset)
    {
        Vector3 top = default, bottom = default;
        
        if (_1x2Block != null)
        {
            top = _1x2Block.transform.position;
            bottom = _1x2Block.transform.position;

            // Lower raycast y position
            top.y -= 0.2f;
            bottom.y -= 0.2f;
            
            if (!IsBlockHorizontal())
            {
                top.z += 0.5f;
                bottom.z -= 0.5f;
            }
            else
            {
                top.x += 0.5f;
                bottom.x -= 0.5f;
            }

        }
        else if (_1x3Block != null)
        {
            top = _1x3Block.transform.position;
            bottom = _1x3Block.transform.position;
            
            // Lower raycast y position
            top.y -= 0.2f;
            bottom.y -= 0.2f;
            
            if (!IsBlockHorizontal())
            {
                top.z += 1f;
                bottom.z -= 1f;
            }
            else
            {
                top.x += 1f;
                bottom.x -= 1f;
            }
        }
        
        #region Top Side
        RaycastHit hitZPos;

        if (!IsBlockHorizontal())
        {
            if (Physics.Raycast(top, Vector3.forward, out hitZPos, _raycastDistance))
            {
                Debug.DrawLine(top, hitZPos.point, Color.blue);

                if (((hitZPos.transform.CompareTag("1x2Block") || hitZPos.transform.CompareTag("Player") || hitZPos.transform.CompareTag("Wall") || hitZPos.transform.CompareTag("1x3Block") && hitZPos.transform.rotation.eulerAngles.y == 90) && Mathf.Abs(hitZPos.transform.position.z - upcomingPosition.z) <= 1) ||
                    (hitZPos.transform.CompareTag("1x3Block") && hitZPos.transform.rotation.eulerAngles.y == 0 && Mathf.Abs(hitZPos.transform.position.z - upcomingPosition.z) <= 2))
                {
                    if (_gameScoreActions.IsMovingTowardsFinishPoint(upcomingPosition, IsBlockHorizontal()) && gameObject.CompareTag("Player"))
                        return false;
                    
                    return true;
                }
            }
            else
            {
                Debug.DrawLine(top, top + Vector3.forward * _raycastDistance, Color.green);
            }
        }
        else
        {
            if (Physics.Raycast(top, Vector3.right, out hitZPos, _raycastDistance))
            {
                Debug.DrawLine(top, hitZPos.point, Color.cyan);

                if (((hitZPos.transform.CompareTag("1x2Block") || hitZPos.transform.CompareTag("Player") || hitZPos.transform.CompareTag("Wall") || hitZPos.transform.CompareTag("1x3Block") && hitZPos.transform.rotation.eulerAngles.y == 0) && Mathf.Abs(hitZPos.transform.position.x - upcomingPosition.x) <= 1) ||
                    (hitZPos.transform.CompareTag("1x3Block") && hitZPos.transform.rotation.eulerAngles.y == 90 && Mathf.Abs(hitZPos.transform.position.x - upcomingPosition.x) <= 2))
                {
                    if (_gameScoreActions.IsMovingTowardsFinishPoint(upcomingPosition, IsBlockHorizontal()) && gameObject.CompareTag("Player"))
                        return false;
                    
                    return true;
                }
            }
            else
            {
                Debug.DrawLine(top, top + Vector3.right * _raycastDistance, Color.green);
            }
        }
        #endregion

        #region Bottom Side
        RaycastHit hitZNeg;

        if (!IsBlockHorizontal())
        {
            if (Physics.Raycast(bottom, -Vector3.forward, out hitZNeg, _raycastDistance))
            {
                Debug.DrawLine(bottom, hitZNeg.point, Color.red);
                if (hitZNeg.transform.position.z == upcomingPositionBottomOffset.z)
                {
                    if (_gameScoreActions.IsMovingTowardsFinishPoint(upcomingPosition, IsBlockHorizontal()) && gameObject.CompareTag("Player"))
                        return false;
                    
                    return true;
                }
            }
            else
            {
                Debug.DrawLine(bottom, bottom - Vector3.forward * _raycastDistance, Color.green);
            }
        }
        else
        {
            if (Physics.Raycast(bottom, -Vector3.right, out hitZNeg, _raycastDistance))
            {
                Debug.DrawLine(bottom, hitZNeg.point, Color.red);
                if (hitZNeg.transform.position.x == upcomingPositionBottomOffset.x)
                {
                    if (_gameScoreActions.IsMovingTowardsFinishPoint(upcomingPosition, IsBlockHorizontal()) && gameObject.CompareTag("Player"))
                        return false;
                    
                    return true;
                }
            }
            else
            {
                Debug.DrawLine(bottom, bottom - Vector3.right * _raycastDistance, Color.green);
            }
        }
        #endregion

        return false;
    }
}