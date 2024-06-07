using Godot;
using HellFarm.Code.Events;

namespace HellFarm.Code.GameOjects;

public partial class ExperienceVial : Node2D
{
	private Area2D _area2D;
	private GameEvents _gameEvents;
	private Vector2 _startPosition;
	
	public override void _Ready()
	{
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D otherArea)
	{
		_startPosition = otherArea.GlobalPosition;
		/*var tween = CreateTween();
		tween.TweenMethod(new Callable(this, nameof(TweenCollect)), 0.0f, 2.0f, 3.1f);
		tween.TweenCallback(new Callable(this, nameof(Collect)));*/
		Collect();
	}

	private void Collect()
	{
		_gameEvents.EmitExperienceVialCollected(1);
		QueueFree();
	}
	
	private void TweenCollect(float percent)
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null)
			return;

		GlobalPosition = _startPosition.Lerp(player.GlobalPosition, percent);
	}
	
}