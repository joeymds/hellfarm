
using System;
using System.Reflection;
using HellFarm.Code.Components;
using Godot;
using Player = HellFarm.Code.Actors.Player;

namespace HellFarm.Code.Managers;

public partial class EnemyManager : Node
{
	private const int SpawnRadius = 350;
	
	[Export] public PackedScene BasicEnemyScene { get; set; }
	[Export] public PackedScene PigEnemyScene { get; set; }
	[Export] public PackedScene ChickenEnemyScene { get; set; }
	[Export] public ArenaTimeManager ArenaTimeManager { get; set; }
	[Export] public Timer BurstWaveTimer { get; set; }
	[Export] public float SpawnRate { get; set; } = 1.0f;
	[Export] public float EliteSpawnChance { get; set; } = 0.05f;
	[Export] public float EliteSpawnChancePerDifficulty { get; set; } = 0.005f;
	[Export] public int EliteUnlockDifficulty { get; set; } = 10;
	[Export] public int EliteFinalSurgeDifficulty { get; set; } = 55;
	[Export] public float EliteFinalSurgeChance { get; set; } = 0.35f;
	[Export] public float MinimumSpawnInterval { get; set; } = 0.3f;
	[Export] public float SpawnRateDifficultyStep { get; set; } = 0.015f;
	[Export] public int BaseMaxActiveEnemies { get; set; } = 30;
	[Export] public int HardMaxActiveEnemies { get; set; } = 60;
	[Export] public int DifficultyPerEnemyCapIncrease { get; set; } = 2;
	[Export] public double BurstWaveDefaultWaitTime { get; set; } = 20.0;
	[Export] public double BurstWaveWaitTimeDifficulty10 { get; set; } = 16.0;
	[Export] public double BurstWaveWaitTimeDifficulty20 { get; set; } = 12.0;
	[Export] public double BurstWaveWaitTimeDifficulty30 { get; set; } = 9.0;
	[Export] public double BurstWaveWaitTimeDifficulty45 { get; set; } = 6.0;
	[Export] public bool EnableDirectionalHerds { get; set; } = true;
	
	
	private Timer _timer;
	private double _baseSpawnTime = 0;
	private Node2D _entitiesLayer;
	private WeightedTable _enemyTable;
	private int _currentArenaDifficulty;
	private bool _pigUnlocked;
	private bool _chickenWeightIncreased;
	private bool _lateGameWeightsApplied;
	private const float DirectionalHerdChance = 0.30f;
	private const float DirectionalHerdSpread = 140.0f;
	private static readonly Color EliteColorModulate = new(1.2f, 1.0f, 0.6f);
	private static readonly Vector2 EliteScale = Vector2.One * 1.3f;
	
	public override void _Ready()
	{
		_enemyTable = new WeightedTable();
		// Spawn progression baseline:
		// - Sheep (BasicEnemy) are always available as the core early enemy.
		// - Chickens are also available from the start as fast/fragile pressure enemies.
		_enemyTable.AddItem(new EnemyItem{ Weight = 10, EnemyScene = BasicEnemyScene });
		_enemyTable.AddItem(new EnemyItem { Weight = 10, EnemyScene = ChickenEnemyScene });
		
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = SpawnRate;
		
		_baseSpawnTime = _timer.WaitTime;
		_entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer") as Node2D;
		if (_entitiesLayer == null)
		{
			GD.PushError("EnemyManager: entities_layer group node was not found. Enemy spawning is disabled.");
		}
		
		_timer.Timeout += OnTimerTimeout;
		BurstWaveTimer ??= GetNodeOrNull<Timer>("BurstWaveTimer");
		if (BurstWaveTimer != null)
		{
			BurstWaveTimer.Timeout += OnBurstWaveTimeout;
		}
		else
		{
			GD.PushWarning("EnemyManager: BurstWaveTimer is not assigned or found; burst waves are disabled.");
		}

		if (ArenaTimeManager != null)
		{
			ArenaTimeManager.ArenaDifficultyIncreased += OnArenaDifficultyIncreased;
		}
		else
		{
			GD.PushError("EnemyManager: ArenaTimeManager is not assigned.");
		}
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

	private bool IsSpawnPathClear(Vector2 fromPosition, Vector2 toPosition)
	{
		var queryParams = PhysicsRayQueryParameters2D.Create(fromPosition, toPosition, 1);
		var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(queryParams);
		return result.Count == 0;
	}
	
	private void OnTimerTimeout()
	{
		_timer.Start();

		if (!CanSpawnEnemy())
			return;
		
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null) return;


		SpawnEnemyAt(GetSpawnPosition());
	}

	private void OnBurstWaveTimeout()
	{
		if (!CanSpawnEnemy())
			return;

		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null) return;

		var burstCount = Random.Shared.Next(3, 6);
		var useDirectionalHerd = EnableDirectionalHerds && GD.Randf() < DirectionalHerdChance;

		if (useDirectionalHerd)
		{
			var cardinalDirection = Random.Shared.Next(0, 4) switch
			{
				0 => Vector2.Up,
				1 => Vector2.Right,
				2 => Vector2.Down,
				_ => Vector2.Left
			};

			var lineCenter = player.GlobalPosition + (cardinalDirection * SpawnRadius);
			var perpendicular = new Vector2(-cardinalDirection.Y, cardinalDirection.X);
			var interval = burstCount <= 1 ? 0f : (DirectionalHerdSpread * 2f) / (burstCount - 1);
			var startOffset = -DirectionalHerdSpread;

			for (var i = 0; i < burstCount; i++)
			{
				if (!CanSpawnEnemy())
					break;

				var alongLineOffset = startOffset + (interval * i);
				var spawnPosition = lineCenter + (perpendicular * alongLineOffset);

				if (!IsSpawnPathClear(player.GlobalPosition, spawnPosition))
				{
					if (IsSpawnPathClear(player.GlobalPosition, lineCenter))
					{
						spawnPosition = lineCenter;
					}
					else
					{
						spawnPosition = GetSpawnPosition();
					}
				}

				SpawnEnemyAt(spawnPosition);
			}

			return;
		}

		var spawnCenter = GetSpawnPosition();

		for (var i = 0; i < burstCount; i++)
		{
			if (!CanSpawnEnemy())
				break;

			var offset = new Vector2((float)GD.RandRange(-20, 20), (float)GD.RandRange(-20, 20));
			SpawnEnemyAt(spawnCenter + offset);
		}
	}

	private bool SpawnEnemyAt(Vector2 position)
	{
		if (!CanSpawnEnemy())
			return false;

		var enemyScene = _enemyTable.PickItem();
		var enemy = enemyScene.EnemyScene.Instantiate() as Node2D;
		if (enemy == null)
			return false;

		if (_entitiesLayer == null)
			return false;

		_entitiesLayer.AddChild(enemy);
		enemy.GlobalPosition = position;

		var scaledEliteChance = GetEliteSpawnChance();

		if (GD.Randf() < scaledEliteChance)
		{
			ApplyEliteModifiers(enemy);
			GD.Print($"EnemyManager: Elite spawned at difficulty {_currentArenaDifficulty} (chance {scaledEliteChance:P0}).");
		}

		return true;
	}

	private static void ApplyEliteModifiers(Node2D enemy)
	{
		enemy.Modulate = EliteColorModulate;
		enemy.Scale = EliteScale;

		if (enemy.HasNode("HealthComponent"))
		{
			var healthComponent = enemy.GetNode<HealthComponent>("HealthComponent");
			if (healthComponent != null)
			{
				healthComponent.MaxHealth *= 1.5f;
				ScaleCurrentHealth(healthComponent, 1.5f);
			}
		}

		if (enemy.HasNode("VelocityComponent"))
		{
			var velocityComponent = enemy.GetNode<VelocityComponent>("VelocityComponent");
			if (velocityComponent != null)
			{
				velocityComponent.MaxSpeed = Mathf.RoundToInt(velocityComponent.MaxSpeed * 1.3f);
			}
		}

		if (enemy.HasNode("VialDropComponent"))
		{
			var vialDropComponent = enemy.GetNode<VialDropComponent>("VialDropComponent");
			if (vialDropComponent != null)
			{
				vialDropComponent.DropPercentage = Mathf.Clamp(vialDropComponent.DropPercentage + 0.15f, 0f, 1f);
			}
		}
	}

	private static void ScaleCurrentHealth(HealthComponent healthComponent, float multiplier)
	{
		var currentHealthProperty = typeof(HealthComponent).GetProperty(
			nameof(HealthComponent.CurrentHealth),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
		);
		var privateSetter = currentHealthProperty?.GetSetMethod(nonPublic: true);
		if (privateSetter == null)
			return;

		var scaledCurrentHealth = healthComponent.CurrentHealth * multiplier;
		privateSetter.Invoke(healthComponent, new object[] { scaledCurrentHealth });
	}
		
	private void OnArenaDifficultyIncreased(int arenaDifficulty)
	{
		_currentArenaDifficulty = arenaDifficulty;
		var scaledSpawnRate = _baseSpawnTime - (SpawnRateDifficultyStep * arenaDifficulty);
		_timer.WaitTime = Math.Max(scaledSpawnRate, MinimumSpawnInterval);

		if (!_pigUnlocked && arenaDifficulty >= 12)
		{
			_enemyTable.AddItem(new EnemyItem { Weight = 20, EnemyScene = PigEnemyScene });
			_pigUnlocked = true;
		}

		if (!_chickenWeightIncreased && arenaDifficulty >= 24)
		{
			_enemyTable.AddItem(new EnemyItem { Weight = 10, EnemyScene = ChickenEnemyScene });
			_chickenWeightIncreased = true;
		}

		if (!_lateGameWeightsApplied && arenaDifficulty >= 36)
		{
			_enemyTable.AddItem(new EnemyItem { Weight = 15, EnemyScene = BasicEnemyScene });
			_enemyTable.AddItem(new EnemyItem { Weight = 10, EnemyScene = PigEnemyScene });
			_lateGameWeightsApplied = true;
		}

		if (BurstWaveTimer != null)
		{
			if (arenaDifficulty >= 45)
			{
				BurstWaveTimer.WaitTime = BurstWaveWaitTimeDifficulty45;
			}
			else if (arenaDifficulty >= 30)
			{
				BurstWaveTimer.WaitTime = BurstWaveWaitTimeDifficulty30;
			}
			else if (arenaDifficulty >= 20)
			{
				BurstWaveTimer.WaitTime = BurstWaveWaitTimeDifficulty20;
			}
			else if (arenaDifficulty >= 10)
			{
				BurstWaveTimer.WaitTime = BurstWaveWaitTimeDifficulty10;
			}
			else
			{
				BurstWaveTimer.WaitTime = BurstWaveDefaultWaitTime;
			}
		}
	}

	private int GetCurrentEnemyCount()
	{
		var enemyNodes = GetTree().GetNodesInGroup("enemy");
		var count = 0;

		foreach (var node in enemyNodes)
		{
			if (node is Node enemyNode && enemyNode.IsInsideTree())
			{
				count++;
			}
		}

		return count;
	}

	private int GetCurrentMaxEnemyCount()
	{
		var capIncrease = DifficultyPerEnemyCapIncrease <= 0 ? 0 : _currentArenaDifficulty / DifficultyPerEnemyCapIncrease;
		return Math.Min(BaseMaxActiveEnemies + capIncrease, HardMaxActiveEnemies);
	}

	private bool CanSpawnEnemy()
	{
		if (_entitiesLayer == null)
			return false;

		return GetCurrentEnemyCount() < GetCurrentMaxEnemyCount();
	}

	private float GetEliteSpawnChance()
	{
		if (_currentArenaDifficulty < EliteUnlockDifficulty)
			return 0f;

		if (_currentArenaDifficulty >= EliteFinalSurgeDifficulty)
			return EliteFinalSurgeChance;

		var levelsSinceUnlock = _currentArenaDifficulty - EliteUnlockDifficulty;
		return EliteSpawnChance + (levelsSinceUnlock * EliteSpawnChancePerDifficulty);
	}
}