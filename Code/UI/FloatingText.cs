using Godot;

namespace HellFarm.Code.UI;

public partial class FloatingText : Node2D
{
    private Label _label;
    
    public override void _Ready()
    {
        
        _label = GetNode<Label>("Label");    
    }

    public void Start(string text)
    {
        _label.Text = text;
    }
    
    public void Kill()
    {
        QueueFree();
    }
}