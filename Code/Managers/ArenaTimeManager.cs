using Godot;
using HellFarm.Code.UI;

namespace HellFarm.Code.Managers;

public partial class ArenaTimeManager : Node
{
	[Export] public PackedScene EndScreenScene { get; set; }
	[Export] public int ArenaDifficulty { get; set; }
	[Export] public int DifficultyInterval { get; set; } = 5;
	
	
	[Signal]
	public delegate void ArenaDifficultyIncreasedEventHandler(int arenaDifficulty);
	
	private Timer _timer;
	
	
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimerTimeout;
	}

	public override void _Process(double delta)
	{
		var nextTimeTarget = (_timer.WaitTime - (ArenaDifficulty + 1) * DifficultyInterval);
		if (_timer.TimeLeft <=  nextTimeTarget)
		{
			ArenaDifficulty += 1;
			EmitSignal("ArenaDifficultyIncreased", ArenaDifficulty);
		}
	}

	private void OnTimerTimeout()
	{
		var victoryScreen = EndScreenScene.Instantiate<EndScreen>();
		AddChild(victoryScreen);
		
	}

	public double GetTimeElapsed()
	{
		return _timer.WaitTime - _timer.TimeLeft;
	}
}