using Godot;
using HellFarm.Code.Components;

namespace HellFarm.Code.Abilities;

public partial class SwordAbility : Node2D
{
    public HitBoxComponent HitBoxComponent;
            
    public override void _Ready()
    { 
        HitBoxComponent = GetNode<HitBoxComponent>("HitBoxComponent");
    }
}