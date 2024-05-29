using Godot;

namespace HellFarm.Code.Events;

public partial class GameEvents : Node
{
	[Signal]
	public delegate void ExperienceVialCollectedEventHandler(int number);
	

	public void EmitExperienceVialCollected(int number)
	{
		EmitSignal("ExperienceVialCollected", number);	
	}
}