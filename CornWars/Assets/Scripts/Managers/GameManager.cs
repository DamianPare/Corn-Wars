using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Lecture_6;
using UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LayerMask BuildingMask;
    [SerializeField] private GameObject editMenu;
    [SerializeField] private GameObject upgradeTab;
    [SerializeField] private TMP_Text editMenuCost;
    [SerializeField] private TMP_Text editMenuReturn;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pathWarning;
    [SerializeField] private GameObject barnWarning;
    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private BuildingPlacementManager _placementManager;
    [SerializeField] private Text CornCounter;
    private GameObject selectedBuilding;
    private BuildingShared building;

    [SerializeField] private int PlayerCount = 1;
    [SerializeField] private BuildingPlacementManager PlacementManager;
    [SerializeField] private UnitManager UnitManager;
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

    public bool isPaused = false;
    private bool isGameOver = false;
    public bool isRoundOver = true;
    private int maxLives = 10;
    private int currentLives;
    private int currentRound = 1;
    private float spawnDelay = 1f;
    private Player player;
    private BuildingShared barn;

    private void Awake()
    {
        currentRound = 1;
        currentLives = maxLives;
        _toggleDelegate += ToggleAllUI;

        PlacementManager.SetGameManager(this);

        for (int i = 0; i < PlayerCount; i++)
        {
            // currently, all players are a different faction for pvp purposes
            player = new Player(i, i, this);
            _playerController.Add(i, player);
            if (i == 0)
            {
                PlacementManager.SetLocalBuildingManager(player.BuildingManager);
                PlayerUI.SubscribeToPlayerUpdates(player);
            }
        }
        _gameGrid = new GameGrid(GridWidth, GridHeight, CellSize, CellTickRate, this);
        UnitManager.SetCellManager(_gameGrid);
    }

    private void Start()
    {
        player.ResourceGain(1000);
        PlayerUI.UpdateLivesUI(currentLives);
    }

    private void Update()
    {
        if (barnWarning.activeSelf && Input.anyKeyDown)
        {
            barnWarning.SetActive(false);
        }

        if (pathWarning.activeSelf && Input.anyKeyDown)
        {
            pathWarning.SetActive(false);
        }

        if (currentLives <= 0)
        {
            GameOver();
        }

        if (UnitManager._enemyUnit.Count <= 0)
        {
            isRoundOver = true;
            PlayerUI.currentRound = currentRound;
            PlayerUI.UpdateRoundUI();
        }

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
                var data = building.GetBuildingInfo();
                EditBuilding(data, building);
            }
        }
    }

    public void StartGame()
    {
        barn = _gameGrid.FindBarn();
        if (barn == null)
        {
            barnWarning.SetActive(true);
            return;
        }

        var path = _gameGrid.Pathfinder.FindShortestPath(PathfindingWithAStar.PathfindingType.AStarManhattan, new Vector3(75, 0, -5), barn.transform.position);
        if (path == null)
        {
            pathWarning.SetActive(true);
            return;
        }

        StartRound();
    }

    public async void StartRound()
    {
        UnitManager.SetBarnLocation(barn);

        Time.timeScale = 1f;
        int roundEnemies = currentRound * 10;
        spawnDelay /= 1.5f;
        isRoundOver = false;

        for (int i = roundEnemies; i > 0; i--)
        {
            UnitManager.SpawnEnemy();
            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }

        currentRound++;
    }

    void EditBuilding(BuildingData data, BuildingShared building)
    {
        var level = building.BuildingLevel;

        if (building.CanLevelUp())
        {
            upgradeTab.SetActive(true);
            editMenuCost.text = $"${data.UpgradeCost[level -1]}";
            editMenuReturn.text = $"${data.SellValue[level - 1]}";
            editMenu.SetActive(true);
        }

        else
        {
            upgradeTab.SetActive(false);
            editMenuReturn.text = $"${data.SellValue[level - 1]}";
            editMenu.SetActive(true);
        }
    }

    public void DeleteBuilding()
    {
        player.BuildingManager.SellBuilding(building.Data, building.BuildingLevel - 1);
        _placementManager.ReleasePooledObject(building);
        editMenu.SetActive(false);
        selectedBuilding = null;
    }

    public void LevelUpBuilding()
    {
        if (building.Data.UpgradeCost[building.BuildingLevel - 1] > player._storedCorn)
        {
            return;
        }

        player.BuildingManager.PurchaseUpgrade(building.Data, building.BuildingLevel - 1);
        editMenu.SetActive(false);
        building.LevelUpBuilding();
        levelUpParticle.gameObject.transform.position = selectedBuilding.transform.position;
        levelUpParticle.Play();
    }

    public void ExitMenu()
    {
        editMenu.SetActive(false);
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

    public void TakeDamage()
    {
        currentLives--;
        PlayerUI.UpdateLivesUI(currentLives);
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
