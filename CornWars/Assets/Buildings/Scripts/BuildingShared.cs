using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingShared : GameManager
{
    [SerializeField] private BuildingData ScriptedObjectData;

    private int CurrentHp;
    private int BuildingLevel;

    protected PlayerBuildingManager _manager;
    protected Player _owner;

    private void Start()
    {
        if (ScriptedObjectData.MaxHp.Length > 0)
        {
            CurrentHp = ScriptedObjectData.MaxHp[0];
        }
        
        BuildingLevel = 1;
    }

    public void SetManager(PlayerBuildingManager manager, ref Action onTick, Player owner)
    {
        _manager = manager;
        onTick += Tick;
        _owner = owner;
    }

    public void CalculateDamage(int damageReceived)
    {
        damageReceived -= ScriptedObjectData.Armor;
        TakeDamage(damageReceived);
    }


    private void TakeDamage(int damageTaken)
    {
        CurrentHp -= damageTaken;
    }

    void CanLevelUp()
    {
        ScriptedObjectData.CanLevelUp(BuildingLevel);
    }

    public override void LevelUpBuilding()
    {
        base.LevelUpBuilding();
        Debug.Log("Hello");
    }

    protected virtual void Tick()
    {
    }
}
