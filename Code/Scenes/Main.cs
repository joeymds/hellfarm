using Godot;
using HellFarm.Code.Actors;
using HellFarm.Code.UI;

namespace HellFarm.Code.Scenes;

public partial class Main : Node
{
	[Export] public PackedScene EndScreenScene { get; set; }
	
	private Player _player;
	
	public override void _Ready()
	{
		_player = GetNode<Player>("%Player");
		_player.HealthComponent.Died += OnPlayerDied;
	}

	private void OnPlayerDied()
	{
		var endScreen = EndScreenScene.Instantiate<EndScreen>();
		AddChild(endScreen);
		endScreen.SetDefeat();
	}
}