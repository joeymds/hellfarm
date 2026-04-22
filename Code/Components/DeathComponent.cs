using Godot;
using HellFarm.Code.Actors;
using HellFarm.Code.Events;

namespace HellFarm.Code.Components;

public partial class DeathComponent : Node
{
	[Export] public int ScoreValue { get; set; } = 1;

	[Export] public HealthComponent HealthComponent { get; set; }
	
	public override void _Ready()
	{
		HealthComponent.Died += OnDeath;
	}

	private void OnDeath()
	{
		var enemy = (IEnemy)GetParent();
		if (enemy != null)
		{
			var gameEvents = GetNode<GameEvents>("/root/GameEvents");
			gameEvents.EmitEnemyKilled(ScoreValue);

			enemy.DeathInitiated();
		}
	}
}