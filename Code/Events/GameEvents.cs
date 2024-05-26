using Godot;

namespace HellFarm.Code.Events;

public partial class GameEvents : Node
{
	[Signal]
	public delegate void ExperienceVialCollectedEventHandler(float number);
	

	public void EmitExperienceVialCollected(float number)
	{
		EmitSignal("ExperienceVialCollected", number);	
	}
}