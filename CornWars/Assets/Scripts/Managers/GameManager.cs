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
    [SerializeField] private BuildingPlacementManager _placementManager;
    private GameObject selectedBuilding;
    private BuildingShared building;

    [SerializeField] private int _gridWidth = 10, _gridHeight = 10, _cellSize = 10;
    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;

    private void Awake()
    {
        _placementManager.SetGameManager(this);

        _gameGrid = new GameGrid(_gridWidth, _gridHeight, _cellSize);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, BuildingMask))
            {
                selectedBuilding = hitInfo.transform.gameObject;
                building = selectedBuilding.GetComponent<BuildingShared>();
                EditBuilding();
            }
        }
    }

    void EditBuilding()
    {
        editMenu.SetActive(true);
    }

    public void DeleteBuilding()
    {
        _placementManager.ReleasePooledObject(building);
        editMenu.SetActive(false);
        selectedBuilding = null;
    }

    public void LevelUpBuilding()
    {
        editMenu.SetActive(false);
        building.LevelUpBuilding();
        //levelUpParticle.gameObject.transform.position = selectedBuilding.transform.position;
        //levelUpParticle.Play();
    }

    public void ExitMenu()
    {
        editMenu.SetActive(false);
    }
}
