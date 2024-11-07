using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CornField : BuildingShared
{
    [SerializeField] private float CropMatureTime = 1f;
    [SerializeField] private int CornPerHarvest = 50;
    [SerializeField] private ParticleSystem harvestParticle;
    [SerializeField] private GameObject firstLevelPrefab;
    [SerializeField] private GameObject secondLevelPrefab;

    private float CropTimer = 0f;
    void Update()
    {
        CropTimer += Time.deltaTime;

        if (CropTimer >= CropMatureTime)
        {
            CropTimer = 0f;
            AddCornToStorage();
        }
    }

    private void OnDisable()
    {
        ResetBuilding();
    }

    private void AddCornToStorage()
    {
        harvestParticle.gameObject.transform.position = this.gameObject.transform.position;
        harvestParticle.Play();
        Debug.Log(Owner);
        Owner.ResourceGain(CornPerHarvest);
        
        //CornTotal += CornPerHarvest;
    }

    public override void LevelUpBuilding()
    {
        firstLevelPrefab.SetActive(false);
        secondLevelPrefab.SetActive(true);
        CornPerHarvest = 100;
    }

    private void ResetBuilding()
    {
        CornPerHarvest = 50;
        firstLevelPrefab.SetActive(true);
        secondLevelPrefab.SetActive(false);
    }

    public override void GetBuildingInfo()
    {
        base.GetBuildingInfo();
    }
}
