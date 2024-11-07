using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Lecture_6;
using UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LayerMask BuildingMask;
    [SerializeField] private GameObject editMenu;
    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private BuildingPlacementManager _placementManager;
    [SerializeField] private Text CornCounter;
    private GameObject selectedBuilding;
    private BuildingShared building;
    private int cornCapital;

    [SerializeField] private int PlayerCount = 1;
    [SerializeField] private BuildingPlacementManager PlacementManager;
    [SerializeField] private LocalPlayerUI PlayerUI;
    [SerializeField] private UnityEvent MyEvent;
    [SerializeField] private Canvas[] UIElements;

    [SerializeField] private int GridWidth = 10;
    [SerializeField] private int GridHeight = 10;
    [SerializeField] private int CellSize = 10;
    [SerializeField] private float CellTickRate;
    [SerializeField] private DamageSystem DamageSystem;

    private SortedList<int, Player> _playerController = new();

    private delegate void DisableUI();
    private DisableUI _toggleDelegate;

    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;
    public DamageSystem CombatSystem => DamageSystem;

    private void Awake()
    {
        _toggleDelegate += ToggleAllUI;

        PlacementManager.SetGameManager(this);

        for (int i = 0; i < PlayerCount; i++)
        {
            // currently, all players are a different faction for pvp purposes
            var player = new Player(i, i, this);
            _playerController.Add(i, player);
            if (i == 0)
            {
                PlacementManager.SetLocalBuildingManager(player.BuildingManager);
                PlayerUI.SubscribeToPlayerUpdates(player);
            }
        }
        _gameGrid = new GameGrid(GridWidth, GridHeight, CellSize, CellTickRate, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _toggleDelegate.Invoke();
        }

        foreach (var p in _playerController.Values)
        {
            p.BuildingManager.OnUpdate();
        }

        _gameGrid.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, BuildingMask))
            {
                selectedBuilding = hitInfo.transform.gameObject;
                building = selectedBuilding.GetComponent<BuildingShared>();
                EditBuilding();
            }
        }
    }

    void EditBuilding()
    {
        editMenu.SetActive(true);
    }

    public void DeleteBuilding()
    {
        _placementManager.ReleasePooledObject(building);
        editMenu.SetActive(false);
        selectedBuilding = null;
    }

    public void LevelUpBuilding()
    {
        editMenu.SetActive(false);
        building.LevelUpBuilding();
        levelUpParticle.gameObject.transform.position = selectedBuilding.transform.position;
        levelUpParticle.Play();
    }

    public void ExitMenu()
    {
        editMenu.SetActive(false);
    }

    private void UpdateCornCounter()
    {

    }

    private void ToggleAllUI()
    {
        foreach (var ui in UIElements)
        {
            ui.enabled = !ui.enabled;
        }
    }

    public Player GetPlayer(int playerIndex)
    {
        Player pReturn;
        if (!_playerController.TryGetValue(playerIndex, out pReturn))
        {
            Debug.LogError($"Tried to get player {playerIndex} but player does not exist!");
        }
        return pReturn;
    }
}
