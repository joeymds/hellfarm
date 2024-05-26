using Actors;
using System;
using System.Linq;
using Godot;
using HellFarm.Code.Abilities;

namespace HellFarm.Code.Controllers;

public partial class SwordAbilityController : Node
{
	[Export] public PackedScene SwordAbility { get; set; }
	[Export] public float MaxRange { get; set; }
	[Export] public float Damage { get; set; } = 5;

	
	private Timer timer;
	
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTimeout;
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

		using var swordInstance = SwordAbility.Instantiate() as SwordAbility;
		player.GetParent().AddChild(swordInstance);
		swordInstance.HitBoxComponent.Damage = Damage;
		swordInstance!.GlobalPosition = enemies[0].GlobalPosition;
		swordInstance.GlobalPosition += Vector2.Right.Rotated((float)(GD.Randfn(0, Mathf.Pi / Mathf.Tau))) * 10;

		var enemyDirection = enemies[0].GlobalPosition - swordInstance.GlobalPosition;
		swordInstance.Rotation = enemyDirection.Angle();
	}

	public override void _Process(double delta)
	{
	}
}