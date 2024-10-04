using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBuildingManager
{
    private Player _owner;

    private List <BuildingShared> _ownedBuildings = new();

    private Action Tick;

    public PlayerBuildingManager(Player owner)
    {
        _owner = owner;
    }

    public void AddBuilding(BuildingShared placedBuilding)
    {
        _ownedBuildings.Add(placedBuilding);
        placedBuilding.SetManager(this, ref Tick, _owner);
    }

    public void OnUpdate()
    {
        Tick?.Invoke();
    }
}
