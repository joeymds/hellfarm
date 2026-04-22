using System.Collections.Generic;
using Godot;
using HellFarm.Code.Managers;

namespace HellFarm.Code.Events;

public partial class GameState: Node
{
    private List<Upgrade> _playerUpgrades;
    private GameEvents _gameEvents;
    
    public List<Upgrade> PlayerUpgrades
    {
        get
        {
            if (_playerUpgrades == null)
            {
                _playerUpgrades = new List<Upgrade>();
            }

            return _playerUpgrades;
        }
        set => _playerUpgrades = value;
    }

    public int Score { get; set; }

    public override void _Ready()
    {
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");
        _gameEvents.EnemyKilled += OnEnemyKilled;
    }

    public override void _ExitTree()
    {
        if (_gameEvents != null)
        {
            _gameEvents.EnemyKilled -= OnEnemyKilled;
        }
    }

    private void OnEnemyKilled(int scoreValue)
    {
        Score += scoreValue;
    }

    public void ResetRunState()
    {
        _playerUpgrades = new List<Upgrade>();
        Score = 0;
    }
}