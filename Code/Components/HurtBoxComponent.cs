using Godot;
using HellFarm.Code.UI;

namespace HellFarm.Code.Components;

public partial class HurtBoxComponent : Area2D
{
    [Export] public HealthComponent HealthComp { get; set; }
    [Export] public PackedScene FloatingText { get; set; }

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D otherArea)
    {
        if (!(otherArea is HitBoxComponent))
            return;
        
        if (HealthComp == null)
            return;
        
        var hitBoxComponent = (HitBoxComponent) otherArea;
        HealthComp.Damage(hitBoxComponent.Damage);

        var floatingText = (FloatingText)FloatingText.Instantiate();
        GetTree().Root.AddChild(floatingText);
        floatingText.GlobalPosition = GlobalPosition;
        floatingText.Start(hitBoxComponent.Damage.ToString());
        
    }
}