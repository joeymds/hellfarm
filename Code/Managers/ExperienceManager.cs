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

	private void OnExperienceVialCollected(int number)
	{
		IncrementExperience(number);
	}

	public void IncrementExperience(int amount)
	{
		//CurrentExperience += Mathf.Min(CurrentExperience + amount, TargetExperience);
		CurrentExperience += amount;
		
		EmitSignal("ExperienceUpdated", CurrentExperience, TargetExperience);

		GD.Print($"{CurrentExperience}/{TargetExperience}");
		
		if (CurrentExperience >= TargetExperience)
		{
			CurrentLevel += 1;
			TargetExperience += TargetExperienceGrowth;
			CurrentExperience = 0;
			
			EmitSignal("ExperienceUpdated", CurrentExperience, TargetExperience);
			EmitSignal("LevelUp", CurrentLevel);
		}
	}
	
}