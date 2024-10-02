using UnityEngine;
using System.Collections.Generic;

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
    private GameObject placedBuilding;
    private Vector3 chosenPlace;
    private Quaternion chosenRotation;

    public AllBuildingData AllBuildings => _allBuildingData;
    
    private BuildingData _buildingToPlace = null;
    private Dictionary<string, GameObject> _ghostObjects = new();
    private GameObject _placementGhost = null;
    private bool _allowPlace = true;

    /// <summary>
    /// Called by the <see cref="BuildingPlacementUI"/>
    /// </summary>
    public void OnNewBuildingSelected(BuildingData building)
    {
        _buildingToPlace = building;
    }

    /// <summary>
    /// If we have a <see cref="_buildingToPlace"/> then show the ghost for it at the mouse position
    /// This will need to calculate where ground is.
    /// </summary>
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
                placedBuilding = _placementGhost;
                chosenPlace = hitInfo.point;
                PlaceBuilding();
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
}
