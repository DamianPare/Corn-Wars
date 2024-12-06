using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
    public class BuildingPoolManager : ObjectPool<BuildingShared>
    {
        public BuildingPoolManager(
            Func<BuildingShared> createFunc,
            Action<BuildingShared> actionOnGet = null,
            Action<BuildingShared> actionOnRelease = null,
            Action<BuildingShared> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {

        }
    }

    public class UnitPoolManager : ObjectPool<CellUnit>
    {
        public UnitPoolManager(
            Func<CellUnit> createFunc,
            Action<CellUnit> actionOnGet = null,
            Action<CellUnit> actionOnRelease = null,
            Action<CellUnit> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {

        }
    }

}


