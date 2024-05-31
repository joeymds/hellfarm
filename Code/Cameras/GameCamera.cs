using Godot;
using Player = HellFarm.Code.Actors.Player;

namespace HellFarm.Code.Cameras;

public partial class GameCamera : Camera2D
{
	private Vector2 targetPosition = Vector2.Zero;
	
	public override void _Ready()
	{
		MakeCurrent();
	}
	
	public override void _Process(double delta)
	{
		AcquireTarget();
		GlobalPosition = GlobalPosition.Lerp(targetPosition, 1.0f - Mathf.Exp((float)-delta * 20));
	}

	private void AcquireTarget()
	{
		var playerNodes =GetTree().GetNodesInGroup("player");
		if (playerNodes.Count > 0)
		{
			var player = (Player)playerNodes[0];
			targetPosition = player.GlobalPosition;
			GlobalPosition = player.GlobalPosition;
		}
	}
}