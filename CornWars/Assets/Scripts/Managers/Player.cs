using System;

public class Player
{
    private int _playerIndex;
    private int _playerFaction;
    private int _currentRound;
    public int PlayerFaction => _playerFaction;

    public float _storedCorn;
    private PlayerBuildingManager _buildingManager;
    public GameManager _gameManager;

    public Action<float> OnPowerChanged;

    public PlayerBuildingManager BuildingManager => _buildingManager;

    public Player(int playerIndex, int playerFaction, GameManager gameManager)
    {
        _playerIndex = playerIndex;
        _playerFaction = playerFaction;
        _storedCorn = 0;

        _gameManager = gameManager;
        _buildingManager = new PlayerBuildingManager(this, gameManager);
    }

    public void ResourceGain(float gain)
    {
        _storedCorn += gain;
        OnPowerChanged?.Invoke(_storedCorn);
    }

    public void ResourceLoss(float loss)
    {
        _storedCorn -= loss;
        OnPowerChanged?.Invoke(_storedCorn);
    }
}
