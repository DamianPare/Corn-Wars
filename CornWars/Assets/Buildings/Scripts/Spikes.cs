using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : BuildingShared
{
    [SerializeField] private int damagePerSecond = 1;

    private void OnCollisionStay(Collision collision)
    {
        //EnemyHealth -= damagePerSecond;
    }

    public void SpikeLevelUp()
    {
        damagePerSecond = 2;
    }
}
