using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LayerMask BuildingMask;
    [SerializeField] private GameObject editMenu;
    [SerializeField] private ParticleSystem levelUpParticle;
    private GameObject selectedBuilding;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, BuildingMask))
            {
                selectedBuilding = hitInfo.transform.gameObject;
                EditBuilding();
            }
        }

        Debug.Log(selectedBuilding);
    }

    void EditBuilding()
    {
        editMenu.SetActive(true);
    }

    public void DeleteBuilding()
    {
        selectedBuilding.SetActive(false);
        editMenu.SetActive(false);
    }

    public virtual void LevelUpBuilding()
    {
        editMenu.SetActive(false);
        levelUpParticle.gameObject.transform.position = selectedBuilding.transform.position;
        levelUpParticle.Play();
    }

    public void ExitMenu()
    {
        editMenu.SetActive(false);
    }
}
