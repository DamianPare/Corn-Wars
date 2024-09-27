using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BuildingShared
{
    [SerializeField] private float Range = 30f;
    [SerializeField] private float Damage = 10f;
    private bool inRange;

    private void Update()
    {
        

        if (inRange == true)
        {
            Fire();
        }
    }

    void Fire()
    {
        //enemyhealth -= Damage;
    }
}
