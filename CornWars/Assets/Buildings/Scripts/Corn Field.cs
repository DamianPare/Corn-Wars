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
    private int CornTotal;
    void Update()
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

    public override void LevelUpBuilding()
    {
        firstLevelPrefab.SetActive(false);
        secondLevelPrefab.SetActive(true);
        CornPerHarvest = 100;
    }
}
