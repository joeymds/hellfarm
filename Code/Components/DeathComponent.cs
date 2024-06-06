using Godot;
using HellFarm.Code.Actors;

namespace HellFarm.Code.Components;

public partial class DeathComponent : Node
{
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
			enemy.DeathInitiated();
		}
	}
}