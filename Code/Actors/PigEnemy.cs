using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Actors;

public partial class PigEnemy : CharacterBody2D
{
    
    [Export] public float MaxSpeed { get; set; } = 55.0f;
    
    private HealthComponent _healthComponent;
    private AnimationTree _animationTree;
    private AnimationPlayer _animationPlayer;
    
    private bool isDead;
    
    public override void _Ready()
    {
        _healthComponent = GetNode<HealthComponent>("HealthComponent");
        _animationTree = GetNode<AnimationTree>("AnimationTree");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        
        isDead = false;
        
        _healthComponent.Died += OnDeath;
    }

    private void OnDeath()
    {
        isDead = true;
        _animationTree.Active = false;
        _animationPlayer.Play("death");
    }
    
    public override void _Process(double delta)
    {
        var direction = GetDirectionToPlayer();
        Velocity = direction * MaxSpeed;
		
        if (Velocity == Vector2.Zero)
            return;
		
        var normalizedVelocity = Velocity.Normalized();
		
        _animationTree.Set("parameters/Chase/blend_position", normalizedVelocity);
		
        if (!isDead)
            MoveAndSlide();
    }
	
	
    private Vector2 GetDirectionToPlayer()
    {
        var playerNode = (Node2D)GetTree().GetFirstNodeInGroup("player");
        if (playerNode != null)
        {
            return (playerNode.GlobalPosition - GlobalPosition).Normalized();
        }
        return Vector2.Zero;
    }

    private void EndOfLife()
    {
        QueueFree();
    }
}