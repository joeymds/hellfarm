using Godot;
using HellFarm.Code.Events;

namespace HellFarm.Code.Managers;

public partial class ExperienceManager : Node
{
	[Signal] public delegate void ExperienceUpdatedEventHandler(float currentExperience, float targetExperience);
	[Signal] public delegate void LevelUpEventHandler(int newLevel);


	private const float TargetExperienceGrowth = 5;
	
	public float CurrentExperience { get; private set; }
	public float CurrentLevel { get; private set; }
	public float TargetExperience { get; private set; } = 2;
	

	private GameEvents _gameEvents;
	
	public override void _Ready()
	{
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_gameEvents.ExperienceVialCollected += OnExperienceVialCollected;
	}

	public override void _ExitTree()
	{
		if (_gameEvents != null)
		{
			_gameEvents.ExperienceVialCollected -= OnExperienceVialCollected;
		}
	}

	private void OnExperienceVialCollected(int number)
	{
		GD.Print($"ExperienceManager: Vial collected, adding {number} experience");
		IncrementExperience(number);
	}

	public void IncrementExperience(int amount)
	{
		CurrentExperience += amount;
		GD.Print($"ExperienceManager: CurrentExperience = {CurrentExperience}, TargetExperience = {TargetExperience}");

		while (CurrentExperience >= TargetExperience)
		{
			CurrentExperience -= TargetExperience;
			CurrentLevel += 1;
			TargetExperience += TargetExperienceGrowth;

			EmitSignal(SignalName.LevelUp, (int)CurrentLevel);
		}

		EmitSignal(SignalName.ExperienceUpdated, CurrentExperience, TargetExperience);
	}
	
}