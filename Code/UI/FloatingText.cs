using Godot;

namespace HellFarm.Code.UI;

public partial class FloatingText : Node2D
{
    private Label _label;
    
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");    
    }

    private void Start(string text)
    {
        _label.Text = text;
        var tween = CreateTween();
        tween.TweenProperty(this, "GlobalPosition", GlobalPosition + (Vector2.Up * 16), .3f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(this, "GlobalPosition", GlobalPosition + (Vector2.Up * 48), .4f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);
    }
}