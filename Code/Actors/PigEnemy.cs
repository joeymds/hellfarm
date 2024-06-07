using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Actors;

public partial class PigEnemy : CharacterBody2D, IEnemy
{
    private VelocityComponent _velocityComponent;
    private AnimationTree _animationTree;
    private AnimationPlayer _animationPlayer;
    
    private bool isDead;
    
    public override void _Ready()
    {
        _velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        _animationTree = GetNode<AnimationTree>("AnimationTree");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        
        isDead = false;
    }
    
    public override void _Process(double delta)
    {
        _velocityComponent.AccelerateToPlayer();
        _velocityComponent.Move(this, isDead);
        
        if (Velocity == Vector2.Zero)
            return;
		
        var normalizedVelocity = Velocity.Normalized();
        _animationTree.Set("parameters/Chase/blend_position", normalizedVelocity);
        
    }

    public void EndOfLife()
    {
        QueueFree();
    }

    public void DeathInitiated()
    {
        isDead = true;
        _animationTree.Active = false;
        _animationPlayer.Play("death");
    }
    
}