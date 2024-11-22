using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using Priority_Queue;
using System.Collections.Generic;
using System.Collections;

public class CellUnit : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 5f;
    private Vector3 endPos;

    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Vector3 target;
    private Vector3 _previousPosition;
    private GameGrid _grid;
    private Vector3 offset;

    private bool canMove;
    private int i;
    private BuildingShared _moveTarget;

    private System.Collections.Generic.IList<Vector2> path;
    public Queue<Vector3> path2 { get; private set; }

    private Animator animator;
    public GameObject testcube;

    /// <summary>
    /// todo Update with max health from scriptable object and only allow health to change via function
    /// </summary>
    private int _health = 20;

    /// <summary>
    /// todo clamp hp to 0 and max hp
    /// </summary>
    /// <param name="change"></param>
    /// 

    public void OnEnable()
    {
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
        if (canMove)
        {
            Move();
            animator.SetTrigger("IsMoving");
        }

        if (!canMove && Input.GetKeyDown(KeyCode.Space))
        {
            canMove = true;
        }
    }

    public void Move()
    {
        float step = Time.deltaTime * MoveSpeed;
        var target = _grid.GetCellPositionFromId(path[i]);
        //_moveTarget = _grid.GetCellWorldCenter(target);
        transform.rotation = Quaternion.LookRotation(target - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        _previousPosition = transform.position;
        _grid.UpdateUnitCell(this, _previousPosition);

        if (transform.position == target && i >= 0)
            i--;
            Instantiate(testcube, target, Quaternion.identity);
            Debug.Log(target);

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
        path2 = new Queue<Vector3>();
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    public void MoveToTarget(Vector3 target)
    {
        if (transform.position != target)
        {
            endPos = target;

            path = _grid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarManhattan, transform.position, endPos);
            i = path.Count - 1;
        }
    }

    public void OnDrawGizmos()
    {
        _grid.Pathfinder.DrawPositions();

        if (path == null)
        {
            return;
        }

        for (int b = i; b > 0; b--)
        {
            Gizmos.color = Color.yellow;
            int size = _grid.CellSize;

            var target = _grid.GetCellPositionFromId(path[b]);
            //target = _grid. (target);

            Gizmos.DrawWireCube(target, Vector3.one * size);
        }
    }
}
