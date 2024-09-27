using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private LayerMask BuildingMask;
    [SerializeField] private GameObject editMenu;
    private GameObject selectedBuilding;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, BuildingMask))
            {
                selectedBuilding = hitInfo.transform.gameObject;
                EditBuilding();
            }
        }

        Debug.Log(selectedBuilding);
    }

    void EditBuilding()
    {
        editMenu.SetActive(true);
    }

    public void DeleteBuilding()
    {
        selectedBuilding.SetActive(false);
        editMenu.SetActive(false);
    }

    public void LevelUpBuilding()
    {
        editMenu.SetActive(false);
    }

    public void ExitMenu()
    {
        editMenu.SetActive(false);
    }


    //[SerializeField] private AllBuildingData BuildingData;

    //private Dictionary<BuildingData.BuildingType, ObjectPool<AllBuildingData>> _pools;

    //private BuildingData _runtimeData;
    //internal BuildingData.EnemyObject[] AllEnemies => _runtimeData.Enemies;

    //internal void InitializePool()
    //{
    //    _runtimeData = Instantiate(AllBuildingData);

    //    _pools = new Dictionary<BuildingData.EnemyType, ObjectPool<EnemyBase>>();
    //    foreach (Buildi.EnemyType enemyType in Enum.GetValues(typeof(BuildingData.EnemyType)))
    //        if (!_pools.ContainsKey(enemyType))
    //            _pools.Add(enemyType,
    //                new ObjectPool<EnemyBase>(
    //                    () => CreatePooledItem(enemyType),
    //                    OnTakeFromPool,
    //                    OnReturnedToPool,
    //                    OnDestroyPoolObject,
    //                    true,
    //                    10,
    //                    100));
    //}

    //internal EnemiesScriptable GetRuntimeData()
    //{
    //    return _runtimeData;
    //}

    //internal void SetRuntimeData(EnemiesScriptable data)
    //{
    //    _runtimeData = data;
    //}

    //private EnemyBase CreatePooledItem(BuildingData.EnemyType t)
    //{
    //    var toCreate = GetEnemy(t);
    //    if (toCreate == null)
    //    {
    //        Debug.LogError("Failed to load Enemy Type: " + t);
    //        return null;
    //    }

    //    var enemy = Instantiate(toCreate.Prefab, transform);
    //    enemy.Init(this, t, toCreate.Data);
    //    return enemy;
    //}

    //private void OnTakeFromPool(EnemyBase enemy)
    //{
    //    enemy.gameObject.SetActive(true);
    //}

    //private void OnReturnedToPool(EnemyBase enemy)
    //{
    //    enemy.gameObject.SetActive(false);
    //}

    //private void OnDestroyPoolObject(EnemyBase enemy)
    //{
    //    Destroy(enemy.gameObject);
    //}

    //internal EnemyBase SpawnEnemy(BuildingData.EnemyType type, BuildingData data)
    //{
    //    var enemy = _pools[type].Get();
    //    enemy.SetData(data);
    //    return enemy;
    //}

    //internal void ReleaseEnemy(BuildingData.EnemyType type, EnemyBase enemy)
    //{
    //    _pools[type].Release(enemy);
    //}
}
