using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Actors
{
	public partial class Player : CharacterBody2D
	{
		[Export] public float AccelerationSmoothing { get; set; } = 25;
		[Export] public float MaxSpeed { get; set; } = 125;

		public HealthComponent HealthComponent;
		
		private bool _isDead;
		private int _numberOfCollidingBodies = 0;
		private Area2D _area2D;
		private Timer _damageIntervalTimer;
		private ProgressBar _healthBar;
		private AnimationPlayer _animationPlayer;
		
		public override void _Ready()
		{
			_area2D = GetNode<Area2D>("CollisionArea2D");
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			HealthComponent = GetNode<HealthComponent>("HealthComponent");
			_damageIntervalTimer = GetNode<Timer>("DamageIntervalTimer");
			_healthBar = GetNode<ProgressBar>("HealthBar");
			
			_area2D.BodyEntered += OnBodyEntered;
			_area2D.BodyExited += OnBodyExited;
			
			HealthComponent.HealthChanged += OnHealthChanged;
			HealthComponent.Died += OnDeath;
			_damageIntervalTimer.Timeout += OnDamageIntervalTimerTimeout;

			_isDead = false;
			
			_animationPlayer.Play("run");
			UpdateHealthDisplay();
		}

		private void OnDeath()
		{
			_animationPlayer.Play("dead");
			_isDead = true;
		}

		private void OnHealthChanged()
		{
			UpdateHealthDisplay();
		}

		private void CheckDealDamage()
		{
			if (_numberOfCollidingBodies == 0 || !_damageIntervalTimer.IsStopped())
				return;
			
			HealthComponent.Damage(1);
			_damageIntervalTimer.Start();
			GD.Print(HealthComponent.CurrentHealth);
		}

		private void UpdateHealthDisplay()
		{
			_healthBar.Value = HealthComponent.GetHealthPercentage();
		}
		
		public override void _Process(double delta)
		{
			var movementVector = GetMovementVector();
			var direction = movementVector.Normalized();
			var targetVelocity = direction * MaxSpeed;
			Velocity = Velocity.Lerp(targetVelocity, 1-Mathf.Exp((float)-delta * AccelerationSmoothing));

			if (!_isDead)
			{
				MoveAndSlide();	
			}
		}

		private Vector2 GetMovementVector()
		{
			var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
			var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
			
			return new Vector2(xMovement, yMovement);	
		}
		
		private void OnDamageIntervalTimerTimeout()
		{
			CheckDealDamage();
		}

		private void OnBodyEntered(Node2D otherBody)
		{
			_numberOfCollidingBodies += 1;
			CheckDealDamage();
		}
		
		private void OnBodyExited(Node2D otherBody)
		{
			_numberOfCollidingBodies -= 1;
		}
		
		private void EndOfLife()
		{
			QueueFree();
		}
	}	
}

