using Godot;
using HellFarm.Code.Managers;

namespace HellFarm.Code.UI;

public partial class ExperienceBar : CanvasLayer
{
    [Export] public ExperienceManager ExperienceMan { get; set; }
    
    
}