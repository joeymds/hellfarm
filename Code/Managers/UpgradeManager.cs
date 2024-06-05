using System.Linq;
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

        if (upgrade.MaxQuantity > 0)
        {
            var currentQuantity = _gameEvents.CurrentUpgrades.Find(u => u.Id == upgrade.Id).Quantity;
            if (currentQuantity == upgrade.MaxQuantity)
            {
                UpgradePool = UpgradePool.Where(u => u.Id != upgrade.Id).ToArray();
            }
        }
        
        _gameEvents.EmitAbilityUpgradeAdded(upgrade);
    }

    private AbilityUpgrade[] PickUpgrade()
    {
        //var chosenUpgrades = new AbilityUpgrade[2];
        var chosenUpgrades = new AbilityUpgrade[UpgradePool.Length];
        var filteredUpgrades = (AbilityUpgrade[])UpgradePool.Clone();
        
        for (var i = 0; i < 2; i++)
        {
            if (filteredUpgrades.Length == 0)
                break;
            
            var chosenUpgrade = filteredUpgrades[GD.RandRange(0, filteredUpgrades.Length - 1)];
            if (chosenUpgrade == null)
                break;
            
            chosenUpgrades[i] = chosenUpgrade;
            filteredUpgrades = filteredUpgrades.Where(u => u.Id != chosenUpgrade.Id).ToArray();
        }

        return chosenUpgrades;
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
    
    private void OnLevelUp(int newlevel)
    {
        if (UpgradePool.Length == 0)
            return;
        
        var chosenUpgrade = UpgradePool[GD.RandRange(0, UpgradePool.Length - 1)];
        if (chosenUpgrade == null)
            return;
        
        var chosenUpgrades = PickUpgrade();
        var upgradeScreenInstance = UpgradeScreenScene.Instantiate<UpgradeScreen>();
        AddChild(upgradeScreenInstance);
        upgradeScreenInstance.SetAbilityUpgrades(chosenUpgrades);
        upgradeScreenInstance.UpgradeSelected += OnUpgradeSelected;
    }
}