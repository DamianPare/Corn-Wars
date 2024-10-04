using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : BuildingShared
{
    [SerializeField] private int damagePerSecond = 1;
    [SerializeField] private GameObject firstLevelPrefab;
    [SerializeField] private GameObject secondLevelPrefab;

    private void OnCollisionStay(Collision collision)
    {
        //EnemyHealth -= damagePerSecond;
    }

    public void SpikeLevelUp()
    {
        damagePerSecond = 2;
        firstLevelPrefab.SetActive(false);
        secondLevelPrefab.SetActive(true);
    }
}
