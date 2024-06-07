
using Godot;
using Player = HellFarm.Code.Actors.Player;

namespace HellFarm.Code.Managers;

public partial class EnemyManager : Node
{
	private const int SpawnRadius = 350;
	
	[Export] public PackedScene BasicEnemyScene { get; set; }
	[Export] public PackedScene PigEnemyScene { get; set; }
	[Export] public ArenaTimeManager ArenaTimeManager { get; set; }
	[Export] public float SpawnRate { get; set; } = 1.0f;
	
	
	private Timer _timer;
	private double _baseSpawnTime = 0;
	private WeightedTable _enemyTable;
	
	public override void _Ready()
	{
		_enemyTable = new WeightedTable();
		_enemyTable.AddItem(new EnemyItem{ Weight = 10, EnemyScene = BasicEnemyScene });
		
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = SpawnRate;
		
		_baseSpawnTime = _timer.WaitTime;
		
		_timer.Timeout += OnTimerTimeout;
		ArenaTimeManager.ArenaDifficultyIncreased += OnArenaDifficultyIncreased;
	}

	private Vector2 GetSpawnPosition()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null)
			return Vector2.Zero;

		var spawnPosition = Vector2.Zero;
		var randomDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
		
		for (var i = 0; i < 4; i++)
		{
			spawnPosition = player.GlobalPosition + (randomDirection * SpawnRadius);
		
			var queryParams = PhysicsRayQueryParameters2D.Create(player.GlobalPosition, spawnPosition,1 );
			var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(queryParams);
			if (result.Count == 0)
				break;
			
			randomDirection = randomDirection.Rotated(Mathf.DegToRad(90));
		}

		return spawnPosition;
	}
	
	private void OnTimerTimeout()
	{
		_timer.Start();
		
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null) return;


		var enemyScene = _enemyTable.PickItem();
		var enemy = enemyScene.EnemyScene.Instantiate() as Node2D;
		var entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer") as Node2D;
		if (entitiesLayer == null) return;
		
		entitiesLayer.AddChild(enemy);
		enemy.GlobalPosition = GetSpawnPosition();
	}
		
	private void OnArenaDifficultyIncreased(int arenaDifficulty)
	{
		var timeOff = (.1 / 12) * arenaDifficulty;
		timeOff = Mathf.Min(timeOff, 0.7);
		
		_timer.WaitTime = _baseSpawnTime - timeOff;

		if (arenaDifficulty == 6)
		{
			_enemyTable.AddItem(new EnemyItem { Weight = 20, EnemyScene = PigEnemyScene });
		}
	}
}