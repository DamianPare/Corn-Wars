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

    public bool CanPlace(BuildingData data)
    {
        if (!_ownedBuildings.ContainsKey(data.KindOfStructure))
        {
            Debug.Log("returning true");
            return true;
        }

        bool n = _ownedBuildings[data.KindOfStructure].Count < data.MaxPlaceable;

        return n;
    }
}
