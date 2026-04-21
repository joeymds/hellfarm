using System.Linq;
using Godot;
using HellFarm.Code.Abilities;
using HellFarm.Code.Actors;
using HellFarm.Code.Events;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Controllers;

public partial class DynamiteAbilityController : Node
{
    [Export] public PackedScene DynamiteScene { get; set; }
    [Export] public float Damage { get; set; } = 10.0f;

    private const float ThrowIntervalSeconds = 4.0f;
    private const float VelocityThreshold = 10.0f;

    private GameState _gameState;
    private GameEvents _gameEvents;
    private Timer _timer;

    private float _baseDamage;
    private Vector2 _lastKnownThrowDirection = Vector2.Down;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");
        _gameEvents.AbilityUpgradeAdded += OnAbilityUpgradeAdded;

        _timer = GetNodeOrNull<Timer>("Timer");
        if (_timer == null)
        {
            _timer = new Timer();
            AddChild(_timer);
        }

        _timer.OneShot = false;
        _timer.Autostart = false;
        _timer.WaitTime = ThrowIntervalSeconds;
        _timer.Timeout += OnTimerTimeout;
        _timer.Start();

        _baseDamage = Damage;
    }

    public override void _ExitTree()
    {
        if (_timer != null)
        {
            _timer.Timeout -= OnTimerTimeout;
        }

        if (_gameEvents != null)
        {
            _gameEvents.AbilityUpgradeAdded -= OnAbilityUpgradeAdded;
        }
    }

    private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade)
    {
        if (upgrade.Id != "dynamite_damage")
            return;

        var quantity = _gameState.PlayerUpgrades
            .Where(x => x.Id == "dynamite_damage")
            .Sum(x => x.Quantity);

        Damage = _baseDamage * (1 + quantity * 0.15f);
    }

    private void OnTimerTimeout()
    {
        if (DynamiteScene == null)
            return;

        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null)
            return;

        var throwDirection = ResolveThrowDirection(player.Velocity);

        var layer = GetTree().GetFirstNodeInGroup("foreground_layer") as Node;
        if (layer == null)
        {
            layer = GetTree().GetFirstNodeInGroup("abilities_layer") as Node;
        }

        if (layer == null)
            return;

        var dynamiteInstance = DynamiteScene.Instantiate<DynamiteAbility>();
        dynamiteInstance.Damage = Damage;
        dynamiteInstance.InitialVelocity = throwDirection * dynamiteInstance.ThrowSpeed;
        layer.AddChild(dynamiteInstance);
        dynamiteInstance.GlobalPosition = player.GlobalPosition;
    }

    private Vector2 ResolveThrowDirection(Vector2 playerVelocity)
    {
        if (playerVelocity.Length() > VelocityThreshold)
        {
            _lastKnownThrowDirection = -playerVelocity.Normalized();
        }

        if (_lastKnownThrowDirection == Vector2.Zero)
        {
            _lastKnownThrowDirection = Vector2.Down;
        }

        return _lastKnownThrowDirection;
    }
}
