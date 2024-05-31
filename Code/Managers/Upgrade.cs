using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Managers;

public class Upgrade
{
    public string Id { get; set; }
    public int Quantity { get; set; }
    public AbilityUpgrade AbilityUpgrade { get; set; }
}