using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornField : BuildingShared
{
    [SerializeField] private float CropMatureTime = 1f;
    [SerializeField] private int CornPerHarvest = 50;
    [SerializeField] private ParticleSystem harvestParticle;
    private float CropTimer = 0f;
    private int CornTotal;
    private void Update()
    {
        CropTimer += Time.deltaTime;

        if (CropTimer >= CropMatureTime)
        {
            CropTimer = 0f;
            AddCornToStorage();
        }
    }

    private void AddCornToStorage()
    {
        harvestParticle.gameObject.transform.position = this.gameObject.transform.position;
        harvestParticle.Play();
        CornTotal += CornPerHarvest;
    }

    public void FarmLevelUp()
    {
        CornPerHarvest = 100;
    }
}
