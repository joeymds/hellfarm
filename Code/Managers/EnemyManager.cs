using Actors;
using Godot;

namespace HellFarm.Code.Managers;

public partial class EnemyManager : Node
{
	private const int SPAWN_RADIUS = 350;
	[Export] public PackedScene BasicEnemyScene { get; set; }
	[Export] public float SpawnRate { get; set; } = 1.0f;
	
	private Timer timer;
	
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.WaitTime = SpawnRate;
		timer.Timeout += OnTimerTimeout;
	}

	private void OnTimerTimeout()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null) return;

		var randomDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
		var spawnPosition = player.GlobalPosition + (randomDirection * SPAWN_RADIUS);

		var enemy = BasicEnemyScene.Instantiate() as BasicEnemy;
		GetParent().AddChild(enemy);
		enemy.GlobalPosition = spawnPosition;

	}
}