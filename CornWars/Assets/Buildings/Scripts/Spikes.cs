using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : BuildingShared
{
    [SerializeField] private int damagePerSecond = 1;
    [SerializeField] private GameObject firstLevelPrefab;
    [SerializeField] private GameObject secondLevelPrefab;


    private void Update()
    {
        
    }

    public override void LevelUpBuilding()
    {
        base.LevelUpBuilding();
        damagePerSecond = 2;
        firstLevelPrefab.SetActive(false);
        secondLevelPrefab.SetActive(true);
    }
}
