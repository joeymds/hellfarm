using Godot;

namespace HellFarm.Code.Components;

public partial class HitBoxComponent : Area2D
{
    public float Damage { get; set; } = 0;
}