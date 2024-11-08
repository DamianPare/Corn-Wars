using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BuildingShared
{
    public override void LevelUpBuilding()
    {
        Data.Walkable = true;
        Manager.LevelUpBuilding(this);
        var renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material.SetColor("Color", Color.yellow);
    }

    public void OnDisable()
    {
        Data.Walkable = true;
    }

    public void OnEnable()
    {
        Data.Walkable = false;
    }
}
