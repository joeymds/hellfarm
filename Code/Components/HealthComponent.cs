using Godot;

namespace HellFarm.Code.Components;

public partial class HealthComponent : Node
{
	[Signal] public delegate void DiedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler();
	
	[Export] public float MaxHealth { get; set; } = 10;

	public float CurrentHealth { get; private set; }


	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void Damage(float damageAmount)
	{
		CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);
		EmitSignal("HealthChanged");
		CallDeferred("CheckDeath");
	}
	
	public float GetHealthPercentage()
	{
		if (MaxHealth <= 0)
			return 0;
		
		return Mathf.Min(CurrentHealth / MaxHealth, 1);
	}

	private void CheckDeath()
	{
		if (CurrentHealth == 0)
		{
			EmitSignal("Died");
			//Owner.QueueFree();
		}
	}
}