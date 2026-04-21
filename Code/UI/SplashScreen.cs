using Godot;

namespace HellFarm.Code.UI;

public partial class SplashScreen : Control
{
	[Export] public string MainScenePath { get; set; } = "res://scenes/main/main.tscn";
	
	private bool _canTransition = false;
	
	public override void _Ready()
	{
		// Small delay before allowing input to prevent accidental skip
		GetTree().CreateTimer(0.5).Timeout += () => _canTransition = true;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (!_canTransition)
			return;
			
		// Check for any button press or key press
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			TransitionToGame();
		}
		else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			TransitionToGame();
		}
		else if (@event is InputEventJoypadButton joypadEvent && joypadEvent.Pressed)
		{
			TransitionToGame();
		}
	}
	
	private void TransitionToGame()
	{
		GetTree().ChangeSceneToFile(MainScenePath);
	}
}
