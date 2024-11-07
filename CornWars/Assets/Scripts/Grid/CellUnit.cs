using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using System;

public class CellUnit : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 5f;
    private Vector3 endPos;

    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Vector3 _moveTarget;
    private Vector3 _previousPosition;
    private GameGrid _grid;
    private Vector3 offset;

    private bool canMove;
    private int i;
    private BuildingShared target;

    private System.Collections.Generic.IList<Vector2> path;

    /// <summary>
    /// todo Update with max health from scriptable object and only allow health to change via function
    /// </summary>
    private int _health = 20;

    /// <summary>
    /// todo clamp hp to 0 and max hp
    /// </summary>
    /// <param name="change"></param>
    public void ChangeHealth(int change)
    {
        _health += change;
        // todo death check
    }

    private void Update()
    {
        if (canMove)
        {
            Move();
        }

        if (!canMove && Input.GetKeyDown(KeyCode.Space))
        {
            canMove = true;
        }
    }

    public void Move()
    {
        //_grid.Pathfinder.UpdateWalkableObstacles(_grid._grid);
        float step = Time.deltaTime * MoveSpeed;
        _moveTarget = _grid.GetCellPositionFromId(path[i]);
        //Vector3 pos = _moveTarget + offset;
        transform.position = Vector3.MoveTowards(transform.position, _moveTarget, step);
        _previousPosition = transform.position;
        _grid.UpdateUnitCell(this, _previousPosition);
        if (transform.position.Equals(_moveTarget) && i >= 0)
            i--;
            Debug.Log(i);

        if (i < 0)
        {
            canMove = false;
        }
    }

    public void Setup(int faction, int unitCounter, GameGrid gameGrid)
    {
        _faction = faction;
        name = $"P{faction}_{name}_{unitCounter}";
        _grid = gameGrid;

        var start = _grid.CellIdFromPosition(transform.position);
        offset = transform.position - _grid.GetCellPositionFromId(start);
        Debug.Log("offset is " + offset);
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    public void MoveToTarget(Vector3 target)
    {
        endPos = target;
        
        path = _grid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarEuclid, transform.position, endPos);
        i = path.Count - 1;
    }
}
