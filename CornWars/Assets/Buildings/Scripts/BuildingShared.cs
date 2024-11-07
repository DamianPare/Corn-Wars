using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingShared : MonoBehaviour
{
    [SerializeField] private BuildingData ScriptedObjectData;

    public BuildingData Data => ScriptedObjectData;

    private int _currentHp;
    private int _buildingLevel;

    public int _placed;

    protected PlayerBuildingManager Manager;
    protected Player Owner;

    private void Start()
    {
        if (ScriptedObjectData.MaxHp.Length > 0)
        {
            _currentHp = ScriptedObjectData.MaxHp[0];
        }
    
        _buildingLevel = 1;
    }

    public void SetManager(PlayerBuildingManager manager, ref Action onTick, Player owner)
    {
        Manager = manager;
        onTick += Tick;
        Owner = owner;
    }

    public int GetFaction()
    {
        return Owner.PlayerFaction;
    }

    public void CalculateDamage(int damageReceived)
    {
        damageReceived -= ScriptedObjectData.Armor;
        TakeDamage(damageReceived);
    }

    private void TakeDamage(int damageTaken)
    {
        _currentHp -= damageTaken;
    }

    public void CanLevelUp()
    {
        ScriptedObjectData.CanLevelUp(_buildingLevel);
    }

    public virtual void LevelUpBuilding()
    {
        
    }

    public virtual void GetBuildingInfo()
    {

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
