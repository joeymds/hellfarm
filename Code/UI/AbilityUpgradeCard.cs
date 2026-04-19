using Godot;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.UI;

public partial class AbilityUpgradeCard : PanelContainer
{
    [Signal]
    public delegate void SelectedEventHandler();
    
    
    private Label _nameLabel;
    private Label _descriptionLabel;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("VBoxContainer/NameLabel");
        _descriptionLabel = GetNode<Label>("VBoxContainer/DescriptionLabel");
        
        GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("left_click"))
        {
            EmitSignal("Selected");
        }
    }

    public void SetAbilityUpgrade(AbilityUpgrade upgrade)
    {
        if (upgrade == null)
            return;

        _nameLabel.Text = upgrade.Name;
        _descriptionLabel.Text = upgrade.Description;
    }
}