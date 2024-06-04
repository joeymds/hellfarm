using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Abilities;

public partial class SickleAbility : Node2D
{

    private const float MaxRadius = 100;
    private Vector2 baseRotation = Vector2.Right;
    
    public HitBoxComponent HitBoxComponent;
    
    public override void _Ready()
    {
        baseRotation = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
        HitBoxComponent = GetNode<HitBoxComponent>("HitBoxComponent");
        
        var tween = CreateTween();
        tween.TweenMethod(new Callable(this, nameof(TweenMethod)), 0.0f, 2.0f, 3.1f);
        tween.TweenCallback(Callable.From(this.QueueFree));
        
    }
    
    private void TweenMethod(float rotations)
    {
        var percent = rotations / 2;
        var currentRadius = percent * MaxRadius;
        var currentDirection = baseRotation.Rotated(rotations * Mathf.Tau);
        
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
            return;
        
        GlobalPosition = player.GlobalPosition + (currentDirection * currentRadius);
    }
}