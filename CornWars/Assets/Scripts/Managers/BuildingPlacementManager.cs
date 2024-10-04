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
    [SerializeField] private AllBuildingData _allBuildingData;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private ParticleSystem placedParticle;

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
        BuildingData dataToUse = GetBuildingData(buildingType);
        BuildingShared newPooledBuilding = Instantiate(_buildingToPlace.BuildingPlacedPrefab, transform);
        
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

    private void ReturnBuildingToPool(BuildingShared building)
    {
        building.gameObject.SetActive(false);
    }


    public void OnNewBuildingSelected(BuildingData building)
    {
        _buildingToPlace = building;
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

            _placementGhost.transform.position = hitInfo.point;

            if (Input.GetKeyDown(placeKey))
            {
                
                chosenPlace = hitInfo.point;

                if(_buildingToPlace != null)
                {
                    GetBuildingFromPool(_buildingToPlace.BuildingPlacedPrefab);
                    PlaceBuilding();
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

    private void PlaceBuilding()
    {
        placedBuilding = Instantiate(_buildingToPlace.BuildingPlacedPrefab, transform);
        placedBuilding.transform.SetPositionAndRotation(chosenPlace, chosenRotation);
        placedParticle.gameObject.transform.position = chosenPlace;
        placedParticle.Play();

        //CreatePoolObject(type);
    }

    private void ClearPlacement()
    {
        _buildingToPlace = null;
        _placementGhost.SetActive(false);
        //ReturnBuildingToPool();
    }

    public void TogglePlacement(bool canPlace)
    {
        _allowPlace = canPlace;
    }
}
