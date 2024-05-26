using Godot;
using System;

namespace Actors
{
	public partial class Player : CharacterBody2D
	{
		[Export] public float AccelerationSmoothing { get; set; } = 25;
		[Export] public float MaxSpeed { get; set; } = 125;
		
		
		public Player()
		{
			
		}
		
		public override void _Process(double delta)
		{
			var movementVector = GetMovementVector();
			var direction = movementVector.Normalized();
			var targetVelocity = direction * MaxSpeed;
			Velocity = Velocity.Lerp(targetVelocity, 1-Mathf.Exp((float)-delta * AccelerationSmoothing));
			
			MoveAndSlide();
		}

		private Vector2 GetMovementVector()
		{
			var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
			var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
			
			return new Vector2(xMovement, yMovement);	
		}
	}	
}

