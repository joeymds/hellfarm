using Godot;

namespace HellFarm.Code.Components;

public partial class VelocityComponent : Node
{
	[Export] public int MaxSpeed { get; set; } = 40;
	[Export] public float Acceleration { get; set; } = 5;

	private Vector2 _velocity = Vector2.Zero;

	public void AccelerateToPlayer()
	{
		var owner = GetOwner<Node2D>();
		if (owner == null)
			return;
		
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null)
			return;
		
		var direction = (player.GlobalPosition - owner.GlobalPosition).Normalized();
		AccelerateInDirection(direction);
	}
	
	public void AccelerateInDirection(Vector2 direction)
	{
		var desiredVelocity = direction * MaxSpeed;
		_velocity = _velocity.Lerp(desiredVelocity, (float)(1 - Mathf.Exp(-Acceleration * GetProcessDeltaTime())));
	}
	
	public void Move(CharacterBody2D characterBody2D, bool isDead)
	{
		if (isDead)
			return;
		
		characterBody2D.Velocity = _velocity;
		characterBody2D.MoveAndSlide();
		_velocity = characterBody2D.Velocity;
	}
	
}