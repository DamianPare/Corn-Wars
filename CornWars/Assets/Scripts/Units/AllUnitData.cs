using UnityEngine;

[CreateAssetMenu(
    fileName = "AllUnitsData",
    menuName = "Create Scriptable Objects/All Units Data")]
public class AllUnitData : ScriptableObject
{
    [SerializeField] private UnitData[] _data;
    public UnitData[] Data => _data;
}

