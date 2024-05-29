using System.Collections.Generic;
using Godot;
using HellFarm.Code.UI;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Managers;

public partial class UpgradeManager : Node
{
    [Export] public ExperienceManager ExperienceMgr { get; set; }
    [Export] public AbilityUpgrade[] UpgradePool { get; set; }
    [Export] public PackedScene UpgradeScreenScene { get; set; }
    
    private Dictionary<string, Dictionary<string, object>> currentUpgrades = new();
    
    public override void _Ready()
    {
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
        var hasUpgrade = currentUpgrades.ContainsKey(upgrade.Id);
        if (!hasUpgrade)
        {
            currentUpgrades[upgrade.Id] = new Dictionary<string, object> {
                {"resource", upgrade},
                {"quantity", 1}
            };
        }
        else
        {
            var upgradeInfoDict = currentUpgrades[upgrade.Id];
            upgradeInfoDict["quantity"] = (int)upgradeInfoDict["quantity"] + 1;
        }
        
        GD.Print(currentUpgrades);
    }
}