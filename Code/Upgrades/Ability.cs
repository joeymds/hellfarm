using Godot;

namespace HellFarm.Code.Upgrades;

public partial class Ability : AbilityUpgrade
{
    [Export] public PackedScene AbilityControllerScene { get; set; }
}