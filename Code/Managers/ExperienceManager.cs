using Godot;
using HellFarm.Code.Events;

namespace HellFarm.Code.Managers;

public partial class ExperienceManager : Node
{
	[Signal] public delegate void ExperienceUpdatedEventHandler(float currentExperience, float targetExperience);

	private const float TargetExperienceGrowth = 5;
	
	public float CurrentExperience { get; private set; }
	public float CurrentLevel { get; private set; }
	public float TargetExperience { get; private set; } = 5;
	

	private GameEvents _gameEvents;
	
	public override void _Ready()
	{
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_gameEvents.ExperienceVialCollected += OnExperienceVialCollected;

	}

	private void OnExperienceVialCollected(float number)
	{
		IncrementExperience(number);
	}

	public void IncrementExperience(float amount)
	{
		CurrentExperience += Mathf.Min(CurrentExperience + amount, TargetExperience);
		
		EmitSignal("ExperienceUpdated", CurrentExperience, TargetExperience);

		GD.Print($"{CurrentExperience}/{TargetExperience}");
		
		if (CurrentExperience >= TargetExperience)
		{
			CurrentLevel += 1;
			TargetExperience += TargetExperienceGrowth;
			CurrentExperience = 0;
			EmitSignal("ExperienceUpdated", CurrentExperience, TargetExperience);
		}
	}
	
}