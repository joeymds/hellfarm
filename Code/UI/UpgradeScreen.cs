using Godot;
using HellFarm.Code.Events;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.UI;

public partial class UpgradeScreen : CanvasLayer
{
    [Export] public PackedScene UpgradeCardScene { get; set; }
    
    [Signal] public delegate void UpgradeSelectedEventHandler(AbilityUpgrade upgrade);


    private GameState _gameState;
    private HBoxContainer _cardContainer;
    
    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        _cardContainer = GetNode<HBoxContainer>("MarginContainer/CardContainer");
        GetTree().Paused = true;
    }

    public void SetAbilityUpgrades(AbilityUpgrade[] upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            var cardInstance = UpgradeCardScene.Instantiate<AbilityUpgradeCard>();
            _cardContainer.AddChild(cardInstance);
            cardInstance.SetAbilityUpgrade(upgrade);
            cardInstance.Selected += () => OnUpgradeSelected(upgrade);
        }
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        EmitSignal("UpgradeSelected", upgrade);
        GetTree().Paused = false;
        QueueFree();
    }
}