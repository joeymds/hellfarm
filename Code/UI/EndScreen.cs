using Godot;

namespace HellFarm.Code.UI;

public partial class EndScreen : Node
{
	private Label _titleLabel;
	private Label _descriptionLabel;
	private Button _restartButton;
	private Button _quitButton;
	
	public override void _Ready()
	{
		GetTree().Paused = true;
	
		_titleLabel = GetNode<Label>("%TitleLabel");
		_descriptionLabel = GetNode<Label>("%DescriptionLabel");
		_restartButton = GetNode<Button>("%RestartButton");
		_quitButton = GetNode<Button>("%QuitButton");
		
		_restartButton.Pressed += OnRestartButtonPressed;
		_quitButton.Pressed += OnQuitButtonOnPressed;
	}

	public void SetDefeat()
	{
		_titleLabel.Text = "Defeat!";
		_descriptionLabel.Modulate = new Color(234, 7, 5, 1);
		_descriptionLabel.Text = "Your farm has taken your soul!";
	}

	private void OnQuitButtonOnPressed()
	{
		GetTree().Quit();
	}

	private void OnRestartButtonPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/main/main.tscn");
	}
}