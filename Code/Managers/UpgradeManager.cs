using System.Collections.Generic;
using Godot;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Managers;

public partial class UpgradeManager : Node
{
    [Export] public ExperienceManager ExperienceMgr { get; set; }
    [Export] public AbilityUpgrade[] UpgradePool { get; set; }
    
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
        
        var hasUpgrade = currentUpgrades.ContainsKey(chosenUpgrade.Id);
        if (!hasUpgrade)
        {
            currentUpgrades[chosenUpgrade.Id] = new Dictionary<string, object> {
                {"resource", chosenUpgrade},
                {"quantity", 1}
            };
        }
        else
        {
            var upgradeInfoDict = currentUpgrades[chosenUpgrade.Id];
            upgradeInfoDict["quantity"] = (int)upgradeInfoDict["quantity"] + 1;
        }
    }
}