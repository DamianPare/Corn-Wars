using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingShared : MonoBehaviour
{
    [SerializeField] private BuildingData ScriptedObjectData;

    private int _buildingLevel;

    public BuildingData Data => ScriptedObjectData;

    public int currentHp;

    public int BuildingLevel => _buildingLevel;

    public int _placed;

    protected PlayerBuildingManager Manager;
    protected Player Owner;

    private void Start()
    {
        if (ScriptedObjectData.MaxHp.Length > 0)
        {
            currentHp = ScriptedObjectData.MaxHp[0];
        }
    
        _buildingLevel = 1;
    }

    public void SetManager(PlayerBuildingManager manager, ref Action onTick, Player owner)
    {
        Manager = manager;
        onTick += Tick;
        Owner = owner;
    }

    private void OnDisable()
    {
        _buildingLevel = 1;
    }

    public int GetFaction()
    {
        return Owner.PlayerFaction;
    }

    public void CalculateDamage(int damageReceived)
    {
        TakeDamage(damageReceived);
    }

    private void TakeDamage(int damageTaken)
    {
        currentHp -= damageTaken;
    }

    public bool CanLevelUp()
    {
        bool n = _buildingLevel < ScriptedObjectData.SellValue.Length;
        return n;
    }

    public virtual void LevelUpBuilding()
    {
        _buildingLevel++;
    }

    public virtual BuildingData GetBuildingInfo()
    {
        return ScriptedObjectData;
    }

    protected virtual void Tick()
    {

    }

    public virtual void OnPlaced() 
    {

    }
    public virtual void OnRemoved() 
    {

    }
}
