using System.Collections.Generic;
using Godot;
using HellFarm.Code.Managers;

namespace HellFarm.Code.Events;

public partial class GameState: Node
{
    private List<Upgrade> _playerUpgrades;
    
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
}