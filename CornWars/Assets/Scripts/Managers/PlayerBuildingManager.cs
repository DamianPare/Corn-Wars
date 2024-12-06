using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBuildingManager
{
    private Player _owner;
    private GameManager _gameManager;

    private Dictionary<BuildingType, List<BuildingShared>> _ownedBuildings = new();

    private Action _tick;

    public PlayerBuildingManager(Player owner, GameManager gameManager)
    {
        _owner = owner;
        _gameManager = gameManager;
    }

    public void AddBuilding(BuildingShared placedBuilding)
    {
        if (!_ownedBuildings.ContainsKey(placedBuilding.Data.KindOfStructure))
        {
            _ownedBuildings.Add(placedBuilding.Data.KindOfStructure, new List<BuildingShared>());
        }

        _ownedBuildings[placedBuilding.Data.KindOfStructure].Add(placedBuilding);

        placedBuilding.SetManager(this, ref _tick, _owner);
        var cellsBuildingIsOn = _gameManager.GameGrid.GetCellsAroundPosition(placedBuilding.transform.position, 0);
        foreach (var cell in cellsBuildingIsOn)
        {
            cell.AddBuildingToCell(placedBuilding);
        }

        placedBuilding.OnPlaced();
    }

    public void RemoveBuilding(BuildingShared placedBuilding)
    {
        _ownedBuildings[placedBuilding.Data.KindOfStructure].Remove(placedBuilding);
        placedBuilding.OnRemoved();

        placedBuilding.SetManager(this, ref _tick, _owner);
        var cellsBuildingIsOn = _gameManager.GameGrid.GetCellsAroundPosition(placedBuilding.transform.position, 0);
        foreach (var cell in cellsBuildingIsOn)
        {
            cell.RemoveBuildingFromCell(placedBuilding);
        }
    }

    public void LevelUpBuilding(BuildingShared placedBuilding)
    {
        placedBuilding.SetManager(this, ref _tick, _owner);
        var cellsBuildingIsOn = _gameManager.GameGrid.GetCellsAroundPosition(placedBuilding.transform.position, 0);
        foreach (var cell in cellsBuildingIsOn)
        {
            cell.UpdateBuildingInCell(placedBuilding);
        }
    }


    public void OnUpdate()
    {
        _tick?.Invoke();
    }


    public void AddBuildingCellEffect(Vector3 position, int range, int unitHealthChange)
    {
        var cells = _gameManager.GameGrid.GetCellsAroundPosition(position, range);
        foreach (var cell in cells)
        {
            cell.ModifyHpChangePerTickInCell(unitHealthChange);
        }
    }

    public void RemoveBuildingCellEffect(Vector3 position, int range, int unitHealthChange)
    {
        var cells = _gameManager.GameGrid.GetCellsAroundPosition(position, range);
        foreach (var cell in cells)
        {
            cell.ModifyHpChangePerTickInCell(-unitHealthChange);
        }
    }

    public bool CanPlace(BuildingData data, Vector3 pos)
    {
        //var barn = _gameManager.GameGrid.Pathfinder.GetBarn();

        //var path = _gameManager.GameGrid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarManhattan, new Vector3(75, 0, -5), barn.transform.position);

        ////if (path == null)
        //{
        //    return false;
        //}

        if (data.Cost > _owner._storedCorn)
        {
            Debug.Log("not enough corn");
            return false;
        }

        if (!_ownedBuildings.ContainsKey(data.KindOfStructure))
        {
            return true;
        }

        bool n = _ownedBuildings[data.KindOfStructure].Count < data.MaxPlaceable;

        return n;
    }

    public void PurchaseBuilding(BuildingData data)
    {
        _owner.ResourceLoss(data.Cost);
    }

    public void PurchaseUpgrade(BuildingData data, int level)
    {
        _owner.ResourceLoss(data.UpgradeCost[level]);
    }

    public void SellBuilding(BuildingData data, int level)
    {
        _owner.ResourceGain(data.SellValue[level]);
    }
}
