using Godot;
using HellFarm.Code.Managers;

namespace HellFarm.Code.UI;

public partial class ArenaTimeUI : CanvasLayer
{
	[Export] public Node ArenaTimeManager { get; set; }

	private Label timeLabel;

	public override void _Ready()
	{
		timeLabel = GetNode<Label>("MarginContainer/Label");
	}

	public override void _Process(double delta)
	{
		var timeManager = (ArenaTimeManager)ArenaTimeManager;
		if (timeManager == null)
			return;
		
		var timeElapsed = timeManager.GetTimeElapsed();
		timeLabel.Text = FormatSecondsToString(timeElapsed);
	}

	private string FormatSecondsToString(double seconds)
	{
		var minutes = Mathf.Floor(seconds / 60);
		var remainingSeconds = seconds - (minutes * 60);
		return $"{minutes:00}:{remainingSeconds:00}";
	}
}