using System.Linq;
using Godot;
using HellFarm.Code.Abilities;
using HellFarm.Code.Events;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.Controllers;

public partial class SwordAbilityController : Node
{
	[Export] public PackedScene SwordAbility { get; set; }
	[Export] public float MaxRange { get; set; }
	[Export] public float Damage { get; set; } = 5;
	
	private GameState _gameState;
	private GameEvents _gameEvents;
	
	private Timer _timer;

	private double baseWaitTime;
	private float baseDamage;
	
	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_gameEvents.AbilityUpgradeAdded += OnAbilityUpgradeAdded;
		
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimerTimeout;
		
		baseWaitTime = _timer.WaitTime;
		baseDamage = Damage;
	}

	private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade)
	{
		if (upgrade.Id != "sword_rate" && upgrade.Id != "rake_damage")
			return;

		if (upgrade.Id == "sword_rate")
		{
			var percentReduction = _gameState.PlayerUpgrades
				.Where(x => x.Id == "sword_rate")
				.Sum(x => x.Quantity * .1);
			
			var newWaitTime = baseWaitTime * (1 - percentReduction);
			
			if (newWaitTime < 0.05)
				newWaitTime = 0.05;
			
			_timer.WaitTime = newWaitTime;
			_timer.Start();
		}

		if (upgrade.Id == "rake_damage")
		{
			var damageIncrease = _gameState.PlayerUpgrades
				.Where(x => x.Id == "rake_damage")
				.Sum(x => x.Quantity * 0.15);

			Damage = (float)(baseDamage * (1 + damageIncrease));
		}
	}

	private void OnTimerTimeout()
	{
		var player = GetTree().GetNodesInGroup("player").FirstOrDefault() as Node2D;
		if (player == null) return;

		var enemies = GetTree().GetNodesInGroup("enemy").Cast<Node2D>().ToList();
		if (enemies.Count == 0) return;

		// filter enemies
		enemies = enemies.Where(x =>
			x.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < Mathf.Pow(MaxRange, 2)).ToList();

		if (enemies.Count == 0) return;

		enemies.Sort((x, y) =>
		{
			var distanceX = x.GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
			var distanceY = y.GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
			return distanceX.CompareTo(distanceY);
		});

		var swordInstance = SwordAbility.Instantiate<SwordAbility>();
		var foregroundLayer = GetTree().GetFirstNodeInGroup("foreground_layer") as Node2D;
		if (foregroundLayer == null) return;
		
		foregroundLayer.AddChild(swordInstance);
		swordInstance.HitBoxComponent.Damage = Damage;
		swordInstance!.GlobalPosition = enemies[0].GlobalPosition;
		swordInstance.GlobalPosition += Vector2.Right.Rotated((float)(GD.Randfn(0, Mathf.Pi / Mathf.Tau))) * 10;

		var enemyDirection = enemies[0].GlobalPosition - swordInstance.GlobalPosition;
		swordInstance.Rotation = enemyDirection.Angle();
	}
}