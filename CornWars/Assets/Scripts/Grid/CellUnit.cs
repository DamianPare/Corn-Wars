using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using Priority_Queue;
using System.Collections.Generic;
using System.Collections;

public class CellUnit : MonoBehaviour
{
    [SerializeField] private UnitData ScriptedObjectData;

    private UnitManager unitManager;

    public UnitData Data => ScriptedObjectData;

    [SerializeField] private float MoveSpeed = 5f;
    public Vector3 endPos;

    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    public Vector3 _moveTarget;
    private Vector3 _previousPosition;
    private GameGrid _grid;
    private Vector3 offset;

    private bool canMove;
    private int i;

    private IList<Vector2> path;

    private Animator animator;
    public GameObject testcube;
    private bool defending;

    private int _health = 20;

    /// <summary>
    /// todo clamp hp to 0 and max hp
    /// </summary>
    /// <param name="change"></param>
    /// 

    public void OnEnable()
    {
        canMove = true;
        animator = gameObject.GetComponent<Animator>();
        Debug.Log("set animator");
    }

    public void ChangeHealth(int change)
    {
        _health += change;
        // todo death check
    }

    private void Update()
    {
        //if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(endPos))
        //{       

        //}

        //if (defending)
        //{
        //    //kill enemies
        //}
    }

    public void Move()
    {
        float step = Time.deltaTime * MoveSpeed;
        var target = _grid.GetCellPositionFromId(path[i]);
        _moveTarget = _grid.GetCellWorldCenter(target);
        transform.rotation = Quaternion.LookRotation(_moveTarget - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, _moveTarget, step);
        _previousPosition = transform.position;
        _grid.UpdateUnitCell(this, _previousPosition);

        if (transform.position == _moveTarget && i >= 0)
        {
            i--;
            Instantiate(testcube, _moveTarget, Quaternion.identity);
        }
    }

    public void Setup(int unitCounter, GameGrid gameGrid)
    {
        name = $"P{name}_{unitCounter}";
        _grid = gameGrid;
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    public void MoveToTarget(Vector3 target)
    {
        endPos = target;

        path = _grid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarManhattan, transform.position, endPos);

        if (transform.position != target && path != null)
        {
            i = path.Count - 1;
            Move();
        }
    }

    public void MoveToEnemy(CellUnit otherUnit)
    {
        // in same cell
        if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(otherUnit.transform.position))
        {
            transform.rotation = Quaternion.LookRotation(otherUnit.transform.position - transform.position);
        }
        else
        {
            // else find path

            var path = _grid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarEuclid, transform.position, otherUnit.transform.position);

            if (path == null)
            {
                Debug.LogWarning("Failed to find path!");
                return;
            }
            var firstNode = path.FirstOrDefault();
            if (firstNode == null)
            {
                Debug.LogWarning("Failed to find node!");
                return;
            }

            transform.rotation = Quaternion.LookRotation(_grid.GetCellPositionFromId(firstNode));
        }

        transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);

        _grid.UpdateUnitCell(this, _previousPosition);

        _previousPosition = transform.position;
    }

    public void OnDrawGizmos()
    {

        if (path == null)
        {
            return;
        }

        _grid.Pathfinder.DrawPositions();

        for (int b = i; b > 0; b--)
        {
            Gizmos.color = Color.yellow;
            int size = _grid.CellSize;

            var target = _grid.GetCellPositionFromId(path[b]);
            _moveTarget = _grid.GetCellWorldCenter(target);

            Gizmos.DrawWireCube(_moveTarget, Vector3.one * size);
        }
    }

    public void Die()
    {
        unitManager._enemyUnit.Remove(this);
        unitManager.ReleasePooledUnit(this, transform.position);
    }

    public void SetManager(UnitManager manager)
    {
        unitManager = manager;
    }
}
