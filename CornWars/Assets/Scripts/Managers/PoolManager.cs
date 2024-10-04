using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
    public class PoolManager : ObjectPool<BuildingShared>
    {
        public PoolManager(
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
}


