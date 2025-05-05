using System.Collections.Generic;
using UnityEngine;

public class BorderActions : MonoBehaviour
{
    [SerializeField] private Vector2 _gridSize;
    [SerializeField] private GameObject _borderOutline;
    [SerializeField] private GameObject _borderPrefab;
    [SerializeField] private GameObject _borderParent;
    [SerializeField] private List<GameObject> _borders;
    
    void Start()
    {
        SetBorders();
    }

    private void SetBorders()
    {
        _borderOutline.transform.localScale = new Vector3(_gridSize.x / 10, _gridSize.y / 10, _gridSize.y / 10);
        
        var leftBorder = InstantiateBorder();
        leftBorder.transform.position = new Vector3(-(_gridSize.x / 2), 0, 0);
        leftBorder.transform.localScale = new Vector3(1, 3, _gridSize.x + 1);

        var rightBorder = InstantiateBorder();
        rightBorder.transform.position = new Vector3(_gridSize.x / 2 + 1, 0, 0);
        rightBorder.transform.localScale = new Vector3(1, 3, _gridSize.x + 1);

        var topBorder = InstantiateBorder();
        topBorder.transform.position = new Vector3(0, 0, _gridSize.y / 2 + 1);
        topBorder.transform.localScale = new Vector3(1, 3, _gridSize.y + 1);
        topBorder.transform.rotation = Quaternion.Euler(0, 90, 0);

        var bottomBorder = InstantiateBorder();
        bottomBorder.transform.position = new Vector3(0, 0, -(_gridSize.y / 2));
        bottomBorder.transform.localScale = new Vector3(1, 3, _gridSize.y + 1);
        bottomBorder.transform.rotation = Quaternion.Euler(0, 90, 0);
        
        AddBorderToList(leftBorder);
        AddBorderToList(rightBorder);
        AddBorderToList(topBorder);
        AddBorderToList(bottomBorder);
    }

    private void AddBorderToList(GameObject border)
    {
        _borders.Add(border);
    }

    private GameObject InstantiateBorder()
    {
        return Instantiate(_borderPrefab, _borderParent.transform);
    }

    public bool IsBorderTouching(Vector3 upcomingPosition)
    {
        return upcomingPosition.x == _borders[0].transform.position.x ||
               upcomingPosition.x == _borders[1].transform.position.x ||
               upcomingPosition.z == _borders[2].transform.position.z ||
               upcomingPosition.z == _borders[3].transform.position.z;
    }
}