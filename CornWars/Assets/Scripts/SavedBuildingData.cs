using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable()]
public class SavedBuildingData
{
    public int CurrentHp;
    public int BuildingLevel;
    public int OwnerId;
    public Vector3 Position;
    public BuildingType KindOfStructure;

    [NonSerialized()] public Player player;
}
