using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShared : MonoBehaviour
{
    [SerializeField] private BuildingData ScriptedObjectData;

    private int CurrentHp;
    private int BuildingLevel;

    private void Start()
    {
        if (ScriptedObjectData.MaxHp.Length > 0)
        {
            CurrentHp = ScriptedObjectData.MaxHp[0];
        }
        
        BuildingLevel = 1;
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

}
