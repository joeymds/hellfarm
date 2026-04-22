using System.Collections.Generic;
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
    private List<AbilityUpgradeCard> _cards = new List<AbilityUpgradeCard>();
    private int _selectedIndex = 0;
    
    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        _cardContainer = GetNode<HBoxContainer>("MarginContainer/CardContainer");
        GetTree().Paused = true;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (_cards.Count == 0)
            return;
            
        if (@event.IsActionPressed("ui_left"))
        {
            _selectedIndex--;
            if (_selectedIndex < 0)
                _selectedIndex = _cards.Count - 1;
            UpdateCardHighlights();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_right"))
        {
            _selectedIndex++;
            if (_selectedIndex >= _cards.Count)
                _selectedIndex = 0;
            UpdateCardHighlights();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_accept") || @event.IsActionPressed("ui_select"))
        {
            if (_selectedIndex >= 0 && _selectedIndex < _cards.Count)
            {
                _cards[_selectedIndex].SelectCard();
            }
            GetViewport().SetInputAsHandled();
        }
    }

    public void SetAbilityUpgrades(List<AbilityUpgrade> upgrades)
    {
        if (upgrades == null || upgrades.Count == 0)
            return;

        _cards.Clear();
        _selectedIndex = 0;

        foreach (var upgrade in upgrades)
        {
            if (upgrade == null)
                continue;

            var cardInstance = UpgradeCardScene.Instantiate<AbilityUpgradeCard>();
            _cardContainer.AddChild(cardInstance);
            cardInstance.SetAbilityUpgrade(upgrade);
            cardInstance.Selected += () => OnUpgradeSelected(upgrade);
            _cards.Add(cardInstance);
        }
        
        // Highlight the first card by default
        if (_cards.Count > 0)
        {
            CallDeferred(nameof(UpdateCardHighlights));
        }
    }
    
    private void UpdateCardHighlights()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].SetHighlighted(i == _selectedIndex);
        }
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        EmitSignal("UpgradeSelected", upgrade);
        GetTree().Paused = false;
        QueueFree();
    }
    
    public void SetSelectedIndex(int index)
    {
        if (index >= 0 && index < _cards.Count)
        {
            _selectedIndex = index;
            UpdateCardHighlights();
        }
    }
}