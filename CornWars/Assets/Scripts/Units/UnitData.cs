using UnityEngine;
using System;
using UnityEngine.UI;

[CreateAssetMenu(
    fileName = "UnitData",
    menuName = "Create Scriptable Objects/Unit Data")]
public class UnitData : ScriptableObject
{
    [SerializeField] private int[] _maxHp;
    [SerializeField] private int[] _maxDamage;

    [SerializeField] private CellUnit _unitPrefab;
    [SerializeField] private Sprite _unitSprite;
    [SerializeField] private UnitType _kindOfUnit;
    [SerializeField] private string _unitName;


    public int placed;

    public int[] MaxHp => _maxHp;
    public int[] MaxDamage => _maxDamage;
    public CellUnit UnitPrefab => _unitPrefab;
    public Sprite UnitSprite => _unitSprite;

    public UnitType KindOfUnit => _kindOfUnit;
    public string UnitName => _unitName;


    public bool CanLevelUp(int currentLevel)
    {
        if (currentLevel <= 0)
            return false;

        bool isMaxLevel = currentLevel < _maxHp.Length;
        return !isMaxLevel;
    }
}

public enum UnitType
{
    None = 0,
    Farm = 1,
    Enemy = 2,
}
