using System.ComponentModel;
using Godot;

namespace HellFarm.Code.Upgrades;

public partial class AbilityUpgrade : Resource
{
    [Export] public string Id { get; set; }
    [Export] public int MaxQuantity { get; set; }
    [Export] public string Name { get; set; }
    
    [Export]
    [Description("The ID of another ability the player needs before this one can be unlocked.")]
    public string? Requires { get; set; }
    
    [Export(PropertyHint.MultilineText)] public string Description { get; set; }
}