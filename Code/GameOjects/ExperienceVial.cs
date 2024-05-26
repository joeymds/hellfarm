using Godot;
using HellFarm.Code.Events;

namespace HellFarm.Code.GameOjects;

public partial class ExperienceVial : Node2D
{
	private Area2D _area2D;
	private GameEvents _gameEvents;
	
	public override void _Ready()
	{
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D otherArea)
	{
		_gameEvents.EmitExperienceVialCollected(1);
		QueueFree();
	}
}