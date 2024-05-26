using Godot;
using HellFarm.Code.Components;

public partial class BasicEnemy : CharacterBody2D
{
	[Export] public float MaxSpeed { get; set; } = 45.0f;

	private HealthComponent _healthComponent;
	private Area2D _area2D;
	

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("HealthComponent");
	}

	public override void _Process(double delta)
	{
		var direction = GetDirectionToPlayer();
		Velocity = direction * MaxSpeed;
		MoveAndSlide();
	}

	private Vector2 GetDirectionToPlayer()
	{
		var playerNode = (Node2D)GetTree().GetFirstNodeInGroup("player");
		if (playerNode != null)
		{
			return (playerNode.GlobalPosition - GlobalPosition).Normalized();
		}
		return Vector2.Zero;
	}
}
