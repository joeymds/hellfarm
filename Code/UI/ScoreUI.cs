using Godot;
using HellFarm.Code.Events;

namespace HellFarm.Code.UI;

public partial class ScoreUI : CanvasLayer
{
	private Label scoreLabel;
	private GameState gameState;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("MarginContainer/Label");
		gameState = GetNode<GameState>("/root/GameState");
		scoreLabel.Text = $"Score: {gameState.Score}";
	}

	public override void _Process(double delta)
	{
		scoreLabel.Text = $"Score: {gameState.Score}";
	}
}
