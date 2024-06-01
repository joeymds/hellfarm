
using Godot;
using Player = HellFarm.Code.Actors.Player;

namespace HellFarm.Code.Managers;

public partial class EnemyManager : Node
{
	private const int SpawnRadius = 350;
	
	[Export] public PackedScene BasicEnemyScene { get; set; }
	[Export] public ArenaTimeManager ArenaTimeManager { get; set; }
	[Export] public float SpawnRate { get; set; } = 1.0f;
	
	
	private Timer _timer;
	private double _baseSpawnTime = 0;
	
	
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = SpawnRate;
		
		_baseSpawnTime = _timer.WaitTime;
		
		_timer.Timeout += OnTimerTimeout;
		ArenaTimeManager.ArenaDifficultyIncreased += OnArenaDifficultyIncreased;
	}

	private void OnTimerTimeout()
	{
		_timer.Start();
		
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null) return;

		var randomDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
		var spawnPosition = player.GlobalPosition + (randomDirection * SpawnRadius);

		var enemy = BasicEnemyScene.Instantiate() as Actors.BasicEnemy;
		var entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer") as Node2D;
		if (entitiesLayer == null) return;
		
		entitiesLayer.AddChild(enemy);
		enemy.GlobalPosition = spawnPosition;
	}
		
	private void OnArenaDifficultyIncreased(int arenaDifficulty)
	{
		var timeOff = (.1 / 12) * arenaDifficulty;
		timeOff = Mathf.Min(timeOff, 0.7);
		
		_timer.WaitTime = _baseSpawnTime - timeOff;
		GD.Print($"timeOff: {timeOff}");
	}
}