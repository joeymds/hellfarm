using System.Linq;
using Godot;
using HellFarm.Code.Abilities;
using HellFarm.Code.Actors;
using HellFarm.Code.Events;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Controllers;

public partial class SickleAbilityController : Node
{
    [Export] public PackedScene SickleScene { get; set; }

    [Export] public float Damage { get; set; } = 10;

    private GameState _gameState;
    private GameEvents _gameEvents;

    private Timer _timer;
    private float baseDamage;
    
    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");
        _gameEvents.AbilityUpgradeAdded += OnAbilityUpgradeAdded;

        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;

        baseDamage = Damage;
    }

    private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade)
    {
        if (upgrade.Id != "sickle_damage")
            return;

        var damageIncrease = _gameState.PlayerUpgrades
            .Where(x => x.Id == "sickle_damage")
            .Sum(x => x.Quantity * 0.10);

        Damage = (float)(baseDamage * (1 + damageIncrease));
        GD.Print($"Sickle Damage: {Damage}");
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