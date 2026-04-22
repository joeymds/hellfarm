using Godot;
using HellFarm.Code.Upgrades;

namespace HellFarm.Code.UI;

public partial class AbilityUpgradeCard : PanelContainer
{
    [Signal]
    public delegate void SelectedEventHandler();
    
    private Label _nameLabel;
    private Label _descriptionLabel;
    private bool _isHighlighted = false;
    
    private StyleBoxFlat _normalStyle;
    private StyleBoxFlat _highlightStyle;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("VBoxContainer/NameLabel");
        _descriptionLabel = GetNode<Label>("VBoxContainer/DescriptionLabel");
        
        // Create normal style (no border)
        _normalStyle = new StyleBoxFlat();
        _normalStyle.BgColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        _normalStyle.BorderWidthLeft = 0;
        _normalStyle.BorderWidthRight = 0;
        _normalStyle.BorderWidthTop = 0;
        _normalStyle.BorderWidthBottom = 0;
        
        // Create highlight style (white border, 3 pixels)
        _highlightStyle = new StyleBoxFlat();
        _highlightStyle.BgColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        _highlightStyle.BorderWidthLeft = 3;
        _highlightStyle.BorderWidthRight = 3;
        _highlightStyle.BorderWidthTop = 3;
        _highlightStyle.BorderWidthBottom = 3;
        _highlightStyle.BorderColor = new Color(1f, 1f, 1f, 1f);
        
        // Set initial style
        AddThemeStyleboxOverride("panel", _normalStyle);
        
        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("left_click"))
        {
            SelectCard();
        }
    }
    
    private void OnMouseEntered()
    {
        // When mouse hovers, highlight this card
        var parent = GetParent()?.GetParent() as UpgradeScreen;
        if (parent != null)
        {
            // Find our index and update selection
            var siblings = GetParent().GetChildren();
            for (int i = 0; i < siblings.Count; i++)
            {
                if (siblings[i] == this)
                {
                    parent.SetSelectedIndex(i);
                    break;
                }
            }
        }
    }
    
    public void SelectCard()
    {
        EmitSignal("Selected");
    }
    
    public void SetHighlighted(bool highlighted)
    {
        _isHighlighted = highlighted;
        AddThemeStyleboxOverride("panel", highlighted ? _highlightStyle : _normalStyle);
    }

    public void SetAbilityUpgrade(AbilityUpgrade upgrade)
    {
        if (upgrade == null)
            return;

        _nameLabel.Text = upgrade.Name;
        _descriptionLabel.Text = upgrade.Description;
    }
}