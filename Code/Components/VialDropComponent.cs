using Godot;

namespace HellFarm.Code.Components;

public partial class VialDropComponent : Node
{
	[Export(PropertyHint.Range, "0,1,0.01")] 
	public float DropPercentage { get; set; } = 0.5f;
	
	[Export] public HealthComponent HealthComp { get; set; }
	[Export] public PackedScene VialScene { get; set; }
	
	public override void _Ready()
	{
		HealthComp.Died += OnDeath;
	}

	private void OnDeath()
	{
		if (GD.Randf() > DropPercentage) return;
		if (VialScene == null) return;
		if(!(Owner is Node2D)) return;
		
		var spawnPosition = ((Node2D)Owner).GlobalPosition;
		var vialInstance = (Node2D)VialScene.Instantiate();
		Owner.GetParent().AddChild(vialInstance);
		vialInstance.GlobalPosition = spawnPosition;
	}
}