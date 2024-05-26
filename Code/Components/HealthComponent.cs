using Godot;

namespace HellFarm.Code.Components;

public partial class HealthComponent : Node
{
	[Signal]
	public delegate void DiedEventHandler();
	
	[Export] public float MaxHealth { get; set; } = 10;

	public float CurrentHealth { get; private set; }


	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void Damage(float damageAmount)
	{
		CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);
		CallDeferred("CheckDeath");
	}

	private void CheckDeath()
	{
		if (CurrentHealth == 0)
		{
			EmitSignal("Died");
			Owner.QueueFree();
		}
	}
}