using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Abilities;

public partial class DynamiteAbility : CharacterBody2D
{
    [Export] public float Damage { get; set; } = 0;
    [Export] public float ExplosionRadius { get; set; } = 60;
    [Export] public float ThrowSpeed { get; set; } = 100;

    public Vector2 InitialVelocity { get; set; } = Vector2.Zero;

    private const float TravelDurationSeconds = 1.0f;
    private const float FuseDurationSeconds = 3.0f;

    private static readonly StringName ThrowAnimationName = "throw";
    private static readonly StringName WaitAnimationName = "wait";
    private static readonly StringName ExplodeAnimationName = "explode";

    private Timer _fuseTimer;
    private HitBoxComponent _hitBoxComponent;
    private AnimationPlayer _animationPlayer;

    private float _elapsedSeconds;
    private bool _hasExploded;
    private bool _hasLanded;

    public override void _Ready()
    {
        _fuseTimer = GetNodeOrNull<Timer>("Timer");
        _hitBoxComponent = GetNodeOrNull<HitBoxComponent>("HitBoxComponent");
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("DynamiteStick/AnimationPlayer");

        if (_fuseTimer == null)
        {
            GD.PushWarning($"{Name}: Timer node is missing. Dynamite will not explode.");
            QueueFree();
            return;
        }

        if (_animationPlayer != null)
        {
            // The dynamite_stick scene autoplays "throw", so we don't need to call Play here
        }
        else
        {
            GD.PushWarning($"{Name}: AnimationPlayer node is missing.");
        }

        ConfigureHitBoxForFuse();

        if (InitialVelocity == Vector2.Zero)
        {
            InitialVelocity = Vector2.Right.Rotated(Rotation) * ThrowSpeed;
        }

        Velocity = InitialVelocity;

        _fuseTimer.OneShot = true;
        _fuseTimer.Autostart = false;
        _fuseTimer.WaitTime = FuseDurationSeconds;
        _fuseTimer.Timeout += OnFuseTimerTimeout;
        _fuseTimer.Start();
    }

    public override void _ExitTree()
    {
        if (_fuseTimer != null)
        {
            _fuseTimer.Timeout -= OnFuseTimerTimeout;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_hasExploded)
            return;

        _elapsedSeconds += (float)delta;

        if (_elapsedSeconds >= TravelDurationSeconds && !_hasLanded)
        {
            _hasLanded = true;
            Velocity = Vector2.Zero;
            
            // Switch to wait animation when landing
            if (_animationPlayer != null && _animationPlayer.HasAnimation(WaitAnimationName))
            {
                _animationPlayer.Play(WaitAnimationName);
            }
        }

        MoveAndSlide();
    }

    public void SetThrowDirection(Vector2 direction)
    {
        if (direction == Vector2.Zero)
            return;

        InitialVelocity = direction.Normalized() * ThrowSpeed;
        Velocity = InitialVelocity;
    }

    private void OnFuseTimerTimeout()
    {
        Explode();
    }

    private void Explode()
    {
        if (_hasExploded)
            return;

        _hasExploded = true;
        Velocity = Vector2.Zero;

        ActivateExplosionHitBox();

        if (_animationPlayer != null && _animationPlayer.HasAnimation(ExplodeAnimationName))
        {
            _animationPlayer.Play(ExplodeAnimationName);
            // Animation will call queue_free when finished
            return;
        }

        if (_animationPlayer == null)
        {
            GD.PushWarning($"{Name}: Missing AnimationPlayer during explosion. Freeing node immediately.");
        }
        else
        {
            GD.PushWarning($"{Name}: Missing 'explode' animation. Freeing node immediately.");
        }

        QueueFree();
    }

    private void ConfigureHitBoxForFuse()
    {
        if (_hitBoxComponent == null)
        {
            GD.PushWarning($"{Name}: HitBoxComponent node is missing.");
            return;
        }

        _hitBoxComponent.Damage = Damage;
        _hitBoxComponent.Monitoring = false;
        _hitBoxComponent.Monitorable = false;

        var collisionShape = _hitBoxComponent.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collisionShape != null)
        {
            collisionShape.Disabled = true;
            if (collisionShape.Shape is CircleShape2D circleShape)
            {
                circleShape.Radius = ExplosionRadius;
            }
        }
    }

    private void ActivateExplosionHitBox()
    {
        if (_hitBoxComponent == null)
            return;

        _hitBoxComponent.Damage = Damage;
        _hitBoxComponent.Monitorable = true;
        _hitBoxComponent.Monitoring = true;

        var collisionShape = _hitBoxComponent.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collisionShape != null)
        {
            collisionShape.Disabled = false;
            if (collisionShape.Shape is CircleShape2D circleShape)
            {
                circleShape.Radius = ExplosionRadius;
            }
        }
    }
}
