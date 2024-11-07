using UnityEngine;
using Pool;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// This is the controller for placing buildings.
/// It receives input via the <see cref="BuildingPlacementUI"/>
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    private PlayerBuildingManager _localPlayerBuildingManager = null;

    [SerializeField] private AllBuildingData _allBuildingData;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private ParticleSystem placedParticle;

    [SerializeField] private GameManager _gameManager;

    public KeyCode placeKey;                                                      
    public KeyCode clearKey;
    private BuildingShared placedBuilding;
    private Vector3 chosenPlace;
    private Quaternion chosenRotation;

    public AllBuildingData AllBuildings => _allBuildingData;
    
    private BuildingData _buildingToPlace = null;
    private Dictionary<string, GameObject> _ghostObjects = new();
    private GameObject _placementGhost = null;
    private bool _allowPlace = true;

    private Dictionary<BuildingType, PoolManager> _placedBuildingPool;
    

    private void Start()
    {
        _placedBuildingPool = new Dictionary<BuildingType, PoolManager>();
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
        {
            _placedBuildingPool.Add(type,
                new PoolManager(
                () => CreatePoolObject(type),
                GetBuildingFromPool,
                ReturnBuildingToPool));
        }
    }

    private BuildingShared CreatePoolObject(BuildingType buildingType)
    {
        BuildingShared newPooledBuilding;
        BuildingData dataToUse = GetBuildingData(buildingType);
        newPooledBuilding = Instantiate(dataToUse.BuildingPlacedPrefab, transform);

        return newPooledBuilding;
    }

    private BuildingData GetBuildingData(BuildingType buildingType)
    {
        return _allBuildingData.Data.FirstOrDefault(buildingData => buildingData.KindOfStructure == buildingType);
    }

    private void GetBuildingFromPool(BuildingShared building)          
    {
        building.gameObject.SetActive(true);
    }

    public void ReturnBuildingToPool(BuildingShared building)
    {
        building.gameObject.SetActive(false);
    }

    public void ReleasePooledObject(BuildingShared building)
    {
        _placedBuildingPool[building.Data.KindOfStructure].Release(building);
        _localPlayerBuildingManager.RemoveBuilding(building);
    }
    public void OnNewBuildingSelected(BuildingData building)
    {
        _buildingToPlace = building;
    }

    internal BuildingShared SpawnBuilding(BuildingType buildingtype)
    {
        var building = _placedBuildingPool[buildingtype].Get();
        building.transform.SetPositionAndRotation(chosenPlace, chosenRotation);

        _localPlayerBuildingManager.AddBuilding(building);

        placedParticle.gameObject.transform.position = chosenPlace;
        placedParticle.Play();

        return building;
    }

    private void Update()
    {
        if (!_allowPlace)
        {
            return;
        }

        if (_buildingToPlace == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, GroundMask))
        {
            if (_placementGhost != null)
            {
                _placementGhost.SetActive(false);

            }

            if (_ghostObjects.TryGetValue(_buildingToPlace.BuildingGhostPrefab.name, out var existingGhost))
            {
                _placementGhost = existingGhost;
                _placementGhost.SetActive(true);
            }

            else
            {
                _placementGhost = Instantiate(_buildingToPlace.BuildingGhostPrefab, transform);
                _placementGhost.transform.SetPositionAndRotation(hitInfo.point, chosenRotation);
                _ghostObjects.Add(_buildingToPlace.BuildingGhostPrefab.name, _placementGhost);
            }

            var pos = _gameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

            _placementGhost.transform.position = pos;

            if (Input.GetKeyDown(placeKey) && _buildingToPlace != null)
            {
                chosenPlace = pos;

                if (_localPlayerBuildingManager.CanPlace(_buildingToPlace))
                {
                    SpawnBuilding(_buildingToPlace.KindOfStructure);
                }
            }

            if (Input.GetKeyDown(clearKey))
            {
                ClearPlacement();
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                chosenRotation *= Quaternion.Euler(0,90,0);
                Debug.Log("Rotated L");
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                chosenRotation *= Quaternion.Euler(0, -90, 0);
                Debug.Log("Rotated R");
            }
        }
    }

    private void ClearPlacement()
    {
        _buildingToPlace = null;
        _placementGhost.SetActive(false);
    }

    public void TogglePlacement(bool canPlace)
    {
        _allowPlace = canPlace;
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void SetLocalBuildingManager(PlayerBuildingManager playerBuildingManager)
    {
        _localPlayerBuildingManager = playerBuildingManager;
    }
}
