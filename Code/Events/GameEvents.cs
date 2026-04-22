using System.Collections.Generic;
using Godot;
using HellFarm.Code.Managers;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Events;

public partial class GameEvents : Node
{
    /*private List<Upgrade> _currentUpgrades;
    
    public List<Upgrade> CurrentUpgrades
    {
        get
        {
            if (_currentUpgrades == null)
            {
                _currentUpgrades = new List<Upgrade>();
            }

            return _currentUpgrades;
        }
        set => _currentUpgrades = value;
    }*/

    [Signal]
    public delegate void ExperienceVialCollectedEventHandler(int number);

    [Signal]
    public delegate void AbilityUpgradeAddedEventHandler(AbilityUpgrade upgrade);

    [Signal]
    public delegate void EnemyKilledEventHandler(int scoreValue);
    
    public void EmitExperienceVialCollected(int number)
    {
        EmitSignal("ExperienceVialCollected", number);
    }

    public void EmitAbilityUpgradeAdded(AbilityUpgrade upgrade)
    {
        EmitSignal("AbilityUpgradeAdded", upgrade);
    }

    public void EmitEnemyKilled(int scoreValue)
    {
        EmitSignal("EnemyKilled", scoreValue);
    }
}