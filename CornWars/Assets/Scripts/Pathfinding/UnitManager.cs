using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;
using System;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    private GameGrid _unitCellManager = null;

    [SerializeField] private GameManager GameManager;
    [SerializeField] private AllUnitData _allUnitData;

    [SerializeField] private CellUnit EnemyUnit;
    [SerializeField] private Transform EnemyParent;
    [SerializeField] private CellUnit FarmUnit;
    [SerializeField] private Transform FarmParent;

    [SerializeField] private int UnitsToTestWith;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform enemyStartPos;
    private Transform barnLocation;

    private int i = 0;

    private Dictionary<UnitType, UnitPoolManager> _unitPool;

    public AllUnitData AllUnit => _allUnitData;

    private bool canMove;

    public List<CellUnit> _farmUnit = new();
    public List<CellUnit> _enemyUnit = new();


    private void Start()
    {

        _unitPool = new Dictionary<UnitType, UnitPoolManager>();
        foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
        {
            _unitPool.Add(type,
                new UnitPoolManager(
                () => CreatePoolObject(type),
                GetUnitFromPool,
                ReturnUnitToPool));
        }
    }

    private CellUnit CreatePoolObject(UnitType unitType)
    {
        i++;
        CellUnit newPooledUnit;
        UnitData dataToUse = GetUnitData(unitType);
        newPooledUnit = Instantiate(dataToUse.UnitPrefab, transform);
        newPooledUnit.Setup(i, GameManager.GameGrid);

        return newPooledUnit;
    }
    private UnitData GetUnitData(UnitType unitType)
    {
        return _allUnitData.Data.FirstOrDefault(unitData => unitData.KindOfUnit == unitType);
    }

    private void GetUnitFromPool(CellUnit unit)
    {
        unit.gameObject.SetActive(true);
    }

    public void ReturnUnitToPool(CellUnit unit)
    {
        unit.gameObject.SetActive(false);
    }

    public void ReleasePooledUnit(CellUnit unit, Vector3 pos)
    {
        var cellId = _unitCellManager.CellIdFromPosition(pos);
        _unitCellManager._grid[cellId].RemoveUnitFromCell(unit);
        _unitPool[unit.Data.KindOfUnit].Release(unit);
    }

    internal CellUnit SpawnUnit(UnitType unitType, Vector3 spawnPos, Quaternion spawnRot, ref List<CellUnit> list)
    {
        var unit = _unitPool[unitType].Get();
        unit.transform.SetPositionAndRotation(spawnPos, spawnRot);
        var cellId = _unitCellManager.CellIdFromPosition(spawnPos);
        _unitCellManager._grid[cellId].AddUnitToCell(unit);

        list.Add(unit);

        unit.SetManager(this);

        return unit;
    }

    public void BuyCornBro()
    {
        SpawnUnit(UnitType.Farm, barnLocation.position, barnLocation.rotation, ref _farmUnit);
    }

    public void SpawnEnemy()
    {
        SpawnUnit(UnitType.Enemy, enemyStartPos.position, enemyStartPos.rotation, ref _enemyUnit);
    }

    private void Update()
    {
        foreach (var unit in _enemyUnit)
        {
            if (TargetCheck(unit, unit.endPos, ref _enemyUnit)) continue;
            if (EnemyCheck(unit)) continue;
            //if (BuildingCheck(unit)) continue;
            // if none, patrol randomly
            unit.MoveToTarget(barnLocation.position);
        }

        foreach (var unit in _farmUnit)
        {
            if (EnemyCheck(unit)) continue;
            if (TargetCheck(unit, enemyStartPos.position, ref _farmUnit)) continue;
            // if none, patrol randomly
            unit.MoveToTarget(enemyStartPos.position);
        }
    }

    private bool TargetCheck(CellUnit unit, Vector3 target, ref List<CellUnit> list)
    {
        if (_unitCellManager.CellIdFromPosition(unit.transform.position) == _unitCellManager.CellIdFromPosition(target))
        {
            //var id = _unitCellManager.CellIdFromPosition(target);
            //_unitCellManager._grid.TryGetValue(id, out var cell);
            //var building = cell.BuildingInCell.GetComponent<BuildingShared>();
            //building.CalculateDamage(1);
            GameManager.TakeDamage();
            list.Remove(unit);
            ReleasePooledUnit(unit, unit.transform.position);
            return true;
        }
        return false;
    }

    private bool EnemyCheck(CellUnit unit)
    {
        // Find enemy within vision range (currently, same cell only)
        CellUnit closestEnemy = GameManager.GameGrid.FindClosestOtherFactionUnit(unit);

        if (closestEnemy != null)
        {
            unit.MoveToEnemy(closestEnemy);
            return true;
        }
        return false;
    }

    private bool BuildingCheck(CellUnit unit)
    {
        // Find closest enemy spawn building
        BuildingShared closestSpawnBuilding = GameManager.GameGrid.FindClosestEnemyResourceBuilding(unit);

        if (closestSpawnBuilding != null)
        {
            unit.MoveToTarget(closestSpawnBuilding.transform.position);
            return true;
        }
        return false;
    }

    public void SetCellManager(GameGrid playerCellManager)
    {
        _unitCellManager = playerCellManager;
    }

    public void SetBarnLocation(BuildingShared location)
    {
        
        barnLocation = location.transform;
    }
}
