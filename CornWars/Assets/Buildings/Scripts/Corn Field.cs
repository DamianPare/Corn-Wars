using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CornField : BuildingShared
{
    [SerializeField] private float CropMatureTime = 2f;
    [SerializeField] private int CornPerHarvest = 10;
    [SerializeField] private int Health = 5;
    [SerializeField] private ParticleSystem harvestParticle;
    [SerializeField] private GameObject firstLevelPrefab;
    [SerializeField] private GameObject secondLevelPrefab;

    private float CropTimer = 0f;

    void Update()
    {
        bool canHarvest = !Owner._gameManager.isRoundOver;

            CropTimer += Time.deltaTime;
        if (CropTimer >= CropMatureTime && canHarvest)
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
        Owner.ResourceGain(CornPerHarvest);
    }

    public override void LevelUpBuilding()
    {
        base.LevelUpBuilding();
        firstLevelPrefab.SetActive(false);
        secondLevelPrefab.SetActive(true);
        CornPerHarvest = 20;
        Health = 10;
    }

    private void ResetBuilding()
    {
        CornPerHarvest = 10;
        firstLevelPrefab.SetActive(true);
        secondLevelPrefab.SetActive(false);
    }
}
