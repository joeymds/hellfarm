using Godot;
using HellFarm.Code.Abilities;
using HellFarm.Code.Actors;

namespace HellFarm.Code.Controllers;

public partial class SickleAbilityController : Node
{
    [Export] public PackedScene SickleScene { get; set; }

    [Export] public float Damage { get; set; } = 10;

    private Timer _timer;
    
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout()
    {
        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null) return;
        
        var foreground = GetTree().GetFirstNodeInGroup("foreground_layer") as Node2D;
        if (foreground == null) return;
        
        var sickleInstance = SickleScene.Instantiate<SickleAbility>();
        foreground.AddChild(sickleInstance);
        sickleInstance.GlobalPosition = player.GlobalPosition;
        sickleInstance.HitBoxComponent.Damage = Damage;

    }
}