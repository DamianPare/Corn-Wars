using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTester : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;

    [SerializeField] private CellUnit FactionUnitA;
    [SerializeField] private Transform UnitParentA;
    [SerializeField] private CellUnit FactionUnitB;
    [SerializeField] private Transform UnitParentB;

    [SerializeField] private int UnitsToTestWith;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    private bool canMove;

    // todo, use pooling
    private List<CellUnit> _unitsListA = new();
    private List<CellUnit> _unitsListB = new();

    private void Start()
    {
        GenerateUnits(0, FactionUnitA, UnitParentA, ref _unitsListA);
        GenerateUnits(1, FactionUnitB, UnitParentB, ref _unitsListB);
    }

    private void GenerateUnits(int faction, CellUnit unitPrefab, Transform parent, ref List<CellUnit> list)
    {
        for (int i = 0; i < UnitsToTestWith; i++)
        {
            CellUnit cellUnit = Instantiate<CellUnit>(unitPrefab, startPos.position, Quaternion.identity, parent);

            cellUnit.Setup(faction, i, GameManager.GameGrid);

            list.Add(cellUnit);
        }
    }

    private void Update()
    {
        foreach (var unit in _unitsListA)
        {
            //if (BuildingCheck(unit)) continue;
            //if (EnemyCheck(unit)) continue;
            // if none, patrol randomly
            unit.MoveToTarget(endPos.position);
        }

        foreach (var unit in _unitsListB)
        {
            //if (BuildingCheck(unit)) continue;
            //if (EnemyCheck(unit)) continue;
            // if none, patrol randomly
            unit.MoveToTarget(endPos.position);
        }
    }


    private bool EnemyCheck(CellUnit unit)
    {
        // Find enemy within vision range (currently, same cell only)
        CellUnit closestEnemy = GameManager.GameGrid.FindClosestOtherFactionUnit(unit);

        if (closestEnemy != null)
        {
            //unit.MoveToEnemy(closestEnemy);
            //return true;
        }
        return false;
    }

    private bool BuildingCheck(CellUnit unit)
    {
        // Find closest enemy spawn building
        BuildingShared closestSpawnBuilding = GameManager.GameGrid.FindClosestEnemySpawnBuilding(unit);

        if (closestSpawnBuilding != null)
        {
            unit.MoveToTarget(closestSpawnBuilding.transform.position);
            return true;
        }
        return false;
    }

    /*
    private void OnDrawGizmos()
    {
        if (GameManager == null || GameManager.GameGrid == null)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        int w = GameManager.GameGrid.Width;
        int h = GameManager.GameGrid.Height;
        int size = GameManager.GameGrid.CellSize;
        float halfSize = size / 2f;

        float posX = 0 - ((w * size) + halfSize);
        for (int i = -w; i <= w + 1; i++)
        {
            float posZ = 0 - ((h * size) + halfSize);
            for (int j = -h; j <= h + 1; j++)
            {
                posZ += size;

                Gizmos.DrawWireCube((new Vector3(posX, 0, posZ)), Vector3.one * size);

                //Debug.Log($"{posX},{posZ}");
            }
            posX += size;
        }

        Gizmos.color = Color.yellow;

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, 20000, GroundMask))
        {
            var pos = GameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

            Gizmos.DrawWireCube(pos, Vector3.one * GameManager.GameGrid.CellSize);
        }
    }
    */
}
