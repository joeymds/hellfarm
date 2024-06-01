using Godot;
using HellFarm.Code.Events;
using HellFarm.Code.UI;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Managers;

public partial class UpgradeManager : Node
{
    [Export] public ExperienceManager ExperienceMgr { get; set; }
    [Export] public AbilityUpgrade[] UpgradePool { get; set; }
    [Export] public PackedScene UpgradeScreenScene { get; set; }
    
    private GameEvents _gameEvents;
    
    public override void _Ready()
    {
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");
        ExperienceMgr.LevelUp += OnLevelUp;    
    }

    private void OnLevelUp(int newlevel)
    {
        var chosenUpgrade = UpgradePool[GD.RandRange(0, UpgradePool.Length - 1)];
        if (chosenUpgrade == null)
            return;
        
        var upgradeScreenInstance = UpgradeScreenScene.Instantiate<UpgradeScreen>();
        AddChild(upgradeScreenInstance);
        upgradeScreenInstance.SetAbilityUpgrades(new[] {chosenUpgrade});
        upgradeScreenInstance.UpgradeSelected += OnUpgradeSelected;
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        ApplyUpgrade(upgrade);
    }

    private void ApplyUpgrade(AbilityUpgrade upgrade)
    {
        if (_gameEvents.CurrentUpgrades.Count > 0)
        {
            var hasUpgrade = _gameEvents.CurrentUpgrades.Find(u => u.Id == upgrade.Id);
            if (hasUpgrade == null)
            {
                InsertUpgrade(upgrade);
            }
            else
            {
                hasUpgrade.Quantity++;
            }
        } 
        else
        {
            InsertUpgrade(upgrade);
        }
        _gameEvents.EmitAbilityUpgradeAdded(upgrade);
    }

    private void InsertUpgrade(AbilityUpgrade upgrade)
    {
        _gameEvents.CurrentUpgrades.Add(new Upgrade
        {
            Id = upgrade.Id,
            Quantity = 1,
            AbilityUpgrade = upgrade
        });
    }
}