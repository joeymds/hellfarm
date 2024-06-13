using Godot;

namespace HellFarm.Code.Components;

public partial class HitFlashComponent : Node
{
    [Export] public HealthComponent HealthCmp { get; set; }
    [Export] public Sprite2D Sprite { get; set; }
    [Export] public Material HitFlashMaterial { get; set; }

    private Tween hitFlashTween;
    
    public override void _Ready()
    {
        Sprite.Material = HitFlashMaterial;
        HealthCmp.HealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged()
    {
        if (hitFlashTween != null && hitFlashTween.IsValid())
        {
            hitFlashTween.Kill();
        }

        (Sprite.Material as ShaderMaterial)?.SetShaderParameter("lerp_percent", 1.0);
        hitFlashTween = CreateTween();
        hitFlashTween.TweenProperty(Sprite.Material, "shader_parameter/lerp_percent", 0.0, .3)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

    }
}