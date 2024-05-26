using Godot;
using HellFarm.Code.Managers;

namespace HellFarm.Code.UI;

public partial class ExperienceBar : CanvasLayer
{
    [Export] public ExperienceManager ExperienceMan { get; set; }
    
    private ProgressBar _experienceBar;

    public override void _Ready()
    {
        _experienceBar = GetNode<ProgressBar>("MarginContainer/ExperienceBar");
        _experienceBar.Value = 0;
        
        ExperienceMan.ExperienceUpdated += OnExperienceUpdated;
    }

    private void OnExperienceUpdated(float currentExperience, float targetExperience)
    {
        var percent = currentExperience / targetExperience;
        _experienceBar.Value = percent;
    }
}