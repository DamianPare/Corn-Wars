using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private BuildingPlacementManager _buildingPlacementManager;
    [SerializeField] private SelectBuildingButton SelectBuildingButton;
    [SerializeField] private Transform ScrollRectContent;
    //private IList<BuildingData> buildings;

    private void Start()
    {
        foreach(var building in _buildingPlacementManager.AllBuildings.Data)
        {
            SelectBuildingButton button = Instantiate(
                SelectBuildingButton, ScrollRectContent);
            button.Setup(building, _buildingPlacementManager);
            //buildings.Add(building);
        }
    }

    public void UpdatePlaced()
    {

    }
}
