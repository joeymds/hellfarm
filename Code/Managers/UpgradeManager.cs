using System.Collections.Generic;
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
    
    private GameState _gameState;
    private GameEvents _gameEvents;
    
    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");

        GD.Print($"UpgradeManager: UpgradePool has {UpgradePool?.Length ?? 0} upgrades");
        if (UpgradePool != null)
        {
            foreach (var upgrade in UpgradePool)
            {
                if (upgrade != null)
                    GD.Print($"  - {upgrade.Id}: {upgrade.Name} (Requires: '{upgrade.Requires}')");
                else
                    GD.Print("  - NULL UPGRADE");
            }
        }
        
        ExperienceMgr.LevelUp += OnLevelUp;    
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        ApplyUpgrade(upgrade);
    }

    private void ApplyUpgrade(AbilityUpgrade upgrade)
    { 
        if (_gameState.PlayerUpgrades.Count > 0)
        {
            var hasUpgrade = _gameState.PlayerUpgrades.Find(u => u.Id == upgrade.Id);
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
            var currentQuantity = _gameState.PlayerUpgrades.Find(u => u.Id == upgrade.Id).Quantity;
            if (currentQuantity == upgrade.MaxQuantity)
            {
                UpgradePool = UpgradePool.Where(u => u.Id != upgrade.Id).ToArray();
            }
        }
        
        _gameEvents.EmitAbilityUpgradeAdded(upgrade);
    }

    private List<AbilityUpgrade> PickUpgrade()
    {
        // We only want to present the player with 2 upgrades
        var chosenUpgrades = new List<AbilityUpgrade>(2);
        var filteredUpgrades = new List<AbilityUpgrade>();
        
        // filter out any upgrades that require an existing upgrade
        foreach (var item in UpgradePool)
        {
            if (item == null)
            {
                GD.Print("PickUpgrade: Filtered out null upgrade entry");
                continue;
            }

            if (string.IsNullOrEmpty(item.Requires))
            {
                filteredUpgrades.Add(item);
            }
            else
            {
                var existingItem = _gameState.PlayerUpgrades.FirstOrDefault(pu => pu.Id == item.Requires);
                if (existingItem != null)
                {
                    filteredUpgrades.Add(item);
                }
                else
                {
                    GD.Print($"PickUpgrade: Filtered out {item.Id} because requirement '{item.Requires}' was not found in player upgrades");
                }
            }
        }

        GD.Print($"PickUpgrade: {filteredUpgrades.Count} upgrades passed filter (from {UpgradePool.Length} total)");
        foreach (var item in filteredUpgrades)
        {
            GD.Print($"  - Available: {item.Id}");
        }
        
        for (var i = 0; i < 2; i++)
        {
            if (filteredUpgrades.Count == 0)
                break;
            
            var chosenUpgrade = filteredUpgrades[GD.RandRange(0, filteredUpgrades.Count - 1)];

            chosenUpgrades.Add(chosenUpgrade);
            filteredUpgrades = filteredUpgrades.Where(f => f.Id != chosenUpgrade.Id).ToList();
        }

        GD.Print($"PickUpgrade: Returning {chosenUpgrades.Count} chosen upgrades");
        foreach (var item in chosenUpgrades)
        {
            GD.Print($"  - Chosen: {item.Id}");
        }

        return chosenUpgrades;
    }

    private void InsertUpgrade(AbilityUpgrade upgrade)
    {
        _gameState.PlayerUpgrades.Add(new Upgrade
        {
            Id = upgrade.Id,
            Quantity = 1,
            AbilityUpgrade = upgrade
        });
    }
    
    private void OnLevelUp(int newlevel)
    {
        if (UpgradePool == null || UpgradePool.Length == 0)
            return;

        var chosenUpgrades = PickUpgrade();
        if (chosenUpgrades.Count == 0)
        {
            GD.Print("No valid upgrades available on level up.");
            return;
        }

        var upgradeScreenInstance = UpgradeScreenScene.Instantiate<UpgradeScreen>();
        AddChild(upgradeScreenInstance);
        upgradeScreenInstance.SetAbilityUpgrades(chosenUpgrades);
        upgradeScreenInstance.UpgradeSelected += OnUpgradeSelected;
    }
}