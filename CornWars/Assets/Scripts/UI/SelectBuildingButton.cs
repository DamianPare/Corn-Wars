using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image BuildingSprite;
    [SerializeField] private TMP_Text BuildingText;

    private BuildingData _data;
    private BuildingPlacementManager _manager;

    public void Setup(BuildingData data, BuildingPlacementManager manager)
    {
        _data = data;
        _manager = manager;
        // setup ui of the button
        BuildingSprite.sprite = data.BuildingSprite;
        BuildingText.text = data.BuildingName;
    }

    public void OnButtonSelected()
    {
        _manager.OnNewBuildingSelected(_data);
    }
}
