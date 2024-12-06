using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BuildingShared
{
    [SerializeField] private float Range = 30f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject firstLevelPrefab;
    [SerializeField] private GameObject secondLevelPrefab;
    [SerializeField] private GameObject thirdLevelPrefab;
    [SerializeField] private ParticleSystem cannonParticles;

    private int level = 1;
    private bool aiming;
    private float shootTimer = 0f;
    private CellUnit target;

    private List<CellUnit> targets = new List<CellUnit>();

    private void Update()
    {
        if (targets.Count == 0)
        {
            aiming = false;
            return;
        }

        if (targets[0] == null)
        {
            targets.Remove(targets[0]);
        }

        if (targets.Count > 0)
        {
            CellUnit frontRunner = targets[0];
            var target = frontRunner.gameObject.transform.position;
            Aim(target);
            aiming = true;
        }

        //if (Physics.Raycast(turret.transform.position, turret.transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, Range, enemies))
        //{
        //    //target = hitInfo.transform.gameObject.GetComponent<CellUnit>();
        //}

        if (aiming)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= fireRate)
            {
                Fire();
                shootTimer = 0f;
            }
        }
    }
    private void OnDisable()
    {
        ResetBuilding();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
             targets.Add(other.GetComponent<CellUnit>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            targets.Remove(other.GetComponent<CellUnit>());
        }
    }

    void Fire()
    {
        cannonParticles.Play();

        if (targets[0] == null)
        {
            targets[1].Die();
            targets.Remove(targets[1]);
            Debug.Log("BANG");
        }

        else
        {
            targets[0].Die();
            targets.Remove(targets[0]);
            Debug.Log("BANG");
        }
    }

    private void Aim(Vector3 target)
    {
        turret.transform.LookAt(target, Vector3.up);
    }

    public override void LevelUpBuilding()
    {
        base.LevelUpBuilding();
        fireRate *= 2;

        if (level == 1)
        {
            //firstLevelPrefab.SetActive(false);
            //secondLevelPrefab.SetActive(true);
        }

        if (level == 2)
        {
            //secondLevelPrefab.SetActive(false);
            //thirdLevelPrefab.SetActive(true);
        }
        
    }

    private void ResetBuilding()
    {
        fireRate = 1f;
        firstLevelPrefab.SetActive(true);
        secondLevelPrefab.SetActive(false);
        thirdLevelPrefab.SetActive(false);
    }
}
