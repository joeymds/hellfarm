using Godot;
using HellFarm.Code.Components;
using HellFarm.Code.Events;
using HellFarm.Code.Upgrades;

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
		private AnimationTree _animationTree;
		private Node _abilities;
		private GameEvents _gameEvents;
		private Vector2 _lastDirection = Vector2.Down;

		public override void _Ready()
		{
			_gameEvents = GetNode<GameEvents>("/root/GameEvents");
			_area2D = GetNode<Area2D>("CollisionArea2D");
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			HealthComponent = GetNode<HealthComponent>("HealthComponent");
			_damageIntervalTimer = GetNode<Timer>("DamageIntervalTimer");
			_healthBar = GetNode<ProgressBar>("HealthBar");
			_abilities = GetNode<Node>("Abilities");

			// Activate the animation tree
			_animationTree.Active = true;

			_area2D.BodyEntered += OnBodyEntered;
			_area2D.BodyExited += OnBodyExited;

			HealthComponent.HealthChanged += OnHealthChanged;
			HealthComponent.Died += OnDeath;
			_damageIntervalTimer.Timeout += OnDamageIntervalTimerTimeout;
			_gameEvents.AbilityUpgradeAdded += OnAbilityUpgradeAdded;

			_isDead = false;

			//_animationPlayer.Play("run");

			UpdateHealthDisplay();
		}

		private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade)
		{
			if (upgrade is not Ability)
				return;

			_abilities.AddChild(((Ability)upgrade).AbilityControllerScene.Instantiate());
		}

		private void OnDeath()
		{
			// Disable animation tree so the AnimationPlayer can play the death clip directly.
			_animationTree.Active = false;
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
		}

		private void UpdateHealthDisplay()
		{
			_healthBar.Value = HealthComponent.GetHealthPercentage();
		}

		public override void _Process(double delta)
		{
			if (_isDead)
				return;

			var movementVector = GetMovementVector();
			var direction = movementVector.Normalized();
			var targetVelocity = direction * MaxSpeed;
			Velocity = Velocity.Lerp(targetVelocity, 1 - Mathf.Exp((float)-delta * AccelerationSmoothing));

			MoveAndSlide();
			UpdateAnimation(movementVector);
		}

		private Vector2 GetMovementVector()
		{
			var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
			var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

			return new Vector2(xMovement, yMovement);
		}

		private void UpdateAnimation(Vector2 movementVector)
		{
			// If moving, update the blend position and remember direction.
			if (movementVector.Length() > 0.1f)
			{
				_lastDirection = movementVector.Normalized();
				_animationTree.Set("parameters/blend_position", _lastDirection);
			}
			else
			{
				_animationTree.Set("parameters/blend_position", Vector2.Zero);
			}
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

