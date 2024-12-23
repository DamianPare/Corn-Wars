using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameGrid
{
    private int _width, _height, _cellSize;
    public int Width => _width;
    public int Height => _height;
    public int CellSize => _cellSize;

    public Dictionary<Vector2, GridCell> _grid = new();

    public Dictionary<Vector2, Vector3> _gridWorldPoints = new();

    public int CellCount => _grid.Count;

    private float _cellTickTimer = 0f;
    private float _cellTickRate;

    private GameManager _gameManager;
    public GameManager Manager => _gameManager;

    private PathfindingWithAStar _pathfinder;
    public PathfindingWithAStar Pathfinder => _pathfinder;

    public GameGrid(int mapWidth, int mapHeight, int cellSize, float tickRate, GameManager gameManager)
    {
        _width = mapWidth;
        _height = mapHeight;
        _cellSize = cellSize;
        _cellTickRate = tickRate;
        _gameManager = gameManager;
        GenerateGrid();
        _pathfinder = new PathfindingWithAStar(this, _grid);
    }

    private void GenerateGrid()
    {
        int w = _width;
        int h = _height;
        int size = _cellSize;
        float halfSize = size / 2f;

        float posX = 0 - ((w * size) + halfSize);
        for (int i = -w; i <= w + 1; i++)
        {
            float posZ = 0 - ((h * size) + halfSize);
            for (int j = -h; j <= h + 1; j++)
            {
                var cellId = new Vector2Int(i, j);

                _grid.Add(cellId, new GridCell(this));
                _gridWorldPoints.Add(cellId, new Vector3(posX, 0, posZ));

                posZ += size;
            }
            posX += size;
        }
    }

    public Vector3 ClampToCellBounds(Vector3 posToClamp)
    {
        return new Vector3(
            Mathf.Clamp(posToClamp.x, _width * -_cellSize, _cellSize + _width * _cellSize),
            posToClamp.y,
            Mathf.Clamp(posToClamp.z, _height * -_cellSize, _cellSize + _height * _cellSize));
    }

    public void UpdateUnitCell(CellUnit unit, Vector3 previousPosition)
    {
        previousPosition = ClampToCellBounds(previousPosition);

        int oldCellX = (int)(previousPosition.x / _cellSize);
        int oldCellZ = (int)(previousPosition.z / _cellSize);

        Vector3 currentPosition = ClampToCellBounds(unit.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        //If it didn't change cell, we are done
        if (oldCellX == cellX && oldCellZ == cellZ)
        {
            return;
        }
        var cellId = new Vector2Int(cellX, cellZ);

        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
        }

        _grid[cellId].AddUnitToCell(unit);
    }

    public Vector3 GetCellWorldCenter(Vector3 originalPos)
    {
        var location = ClampToCellBounds(originalPos);
        
        return _gridWorldPoints[CellIdFromPosition(location)];
    }

    public CellUnit FindClosestOtherFactionUnit(CellUnit unitSearching)
    {
        Vector3 currentPosition = ClampToCellBounds(unitSearching.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector2Int(cellX, cellZ);

        // validate this cell has even been registered
        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
            return null;
        }

        CellUnit closestEnemy = null;

        // check again all other units in our grid cell
        var otherUnitsList = _grid[cellId].GetOtherFactionUnits(unitSearching.Faction);

        float smallestDistance = Mathf.Infinity;

        foreach (var otherUnit in otherUnitsList)
        {
            float distSqr = (otherUnit.transform.position - unitSearching.transform.position).sqrMagnitude;

            if (distSqr < smallestDistance)
            {
                smallestDistance = distSqr;
                closestEnemy = otherUnit;
            }
        }

        return closestEnemy;
        // we could also check the surrounding grid cells
    }

    public Vector2Int CellIdFromPosition(Vector3 position)
    {
        Vector3 location = ClampToCellBounds(position);

        int cellX;// = (int)(currentPosition.x / _cellSize);
        int cellZ;// = (int)(currentPosition.z / _cellSize);

        if (location.x < 0)
        {
            cellX = Mathf.CeilToInt((location.x / _cellSize));
        }
        else
        {
            cellX = Mathf.CeilToInt(location.x / _cellSize);

        }

        if (location.z < 0)
        {
            cellZ = Mathf.CeilToInt(location.z / _cellSize);
        }
        else
        {
            cellZ = Mathf.CeilToInt(location.z / _cellSize);
        }

        return new Vector2Int(cellX, cellZ);
    }

    public Vector3 GetCellPositionFromId(Vector2 cellId)
    {
        int cellX = (int)(cellId.x * _cellSize);
        int cellZ = (int)(cellId.y * _cellSize);

        return new Vector3(cellX, 0, cellZ);
    }

    public BuildingShared FindClosestEnemyResourceBuilding(CellUnit unitSearching)
    {
        BuildingShared closestFarmBuilding = null;

        // todo, optimize to check a list of buildings instead of the entire grid
        foreach (var cell in _grid)
        {
            var building = cell.Value.BuildingInCell;
            if (building != null &&
                building is CornField)
            {
                closestFarmBuilding = building;
            }
        }
        return closestFarmBuilding;
    }

    public BuildingShared FindBarn()
    {
        var barn = _pathfinder.GetBarn();

        return barn;
    }

    public void OnUpdate()
    {
        _cellTickTimer += Time.deltaTime;

        if (_cellTickTimer >= _cellTickRate)
        {
            _cellTickTimer = 0f;
            TickAllGrids();
        }
    }

    private void TickAllGrids()
    {
        foreach (GridCell grid in _grid.Values)
        {
            grid.OnTick();
        }
    }

    private GridCell GetCellAtPosition(Vector3 position)
    {
        Vector3 currentPosition = ClampToCellBounds(position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector2Int(cellX, cellZ);

        // validate this cell has even been registered
        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
        }
        return _grid[cellId];
    }

    public List<GridCell> GetCellsAroundPosition(Vector3 position, int range)
    {
        List<GridCell> cells = new();

        Vector3 centeredPosition = ClampToCellBounds(position);

        int cellX = (int)(centeredPosition.x / _cellSize);
        int cellZ = (int)(centeredPosition.z / _cellSize);

        int minX = Mathf.Clamp(cellX - range, -Width, Width);
        int maxX = Mathf.Clamp(cellX + range, minX, Width);
        int minZ = Mathf.Clamp(cellZ - range, -Height, Height);
        int maxZ = Mathf.Clamp(cellZ + range, minZ, Height);

        Debug.Log($"Getting cells between: {minX} to {maxX} horizontal and {minZ} to {maxZ} vertical");
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                var cellId = new Vector2(x, z);

                if (!_grid.ContainsKey(cellId))
                {
                    _grid.Add(cellId, new GridCell(this));
                }

                if (!cells.Contains(_grid[cellId]))
                {
                    cells.Add(GetCellAtPosition(cellId));
                }
            }
        }

        return cells;
    }
}