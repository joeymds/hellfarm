using Godot;

namespace HellFarm.Code.Managers;

public partial class ArenaTimeManager : Node
{
	private Timer timer;
	
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTimeout;
	}

	private void OnTimerTimeout()
	{
		
	}

	public double GetTimeElapsed()
	{
		return timer.WaitTime - timer.TimeLeft;
	}
}