using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public List<Skill> Skills = new List<Skill>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadSkills();
    }

    void LoadSkills()
    {
        Skills.Add(new Skill { ID = 1, Name = "Diagonal Arrow", Description = "Diagonal arrow description", Icon = "DiagonalArrow.png", Effect = Skill.SkillEffect.DiagonalArrow });
        Skills.Add(new Skill { ID = 2, Name = "Fire Arrow", Description = "Fire arrow description", Icon = "FireArrow.png", Effect = Skill.SkillEffect.FireArrow });
        Skills.Add(new Skill { ID = 3, Name = "Frost Arrow", Description = "Frost arrow description", Icon = "IceArrow.png", Effect = Skill.SkillEffect.FrostArrow });
        Skills.Add(new Skill { ID = 4, Name = "Attack Power Boost", Description = "Attack power boost description", Icon = "ATK_UP.png", Effect = Skill.SkillEffect.AttackBoost });
        Skills.Add(new Skill { ID = 5, Name = "Movement Speed Boost", Description = "Movement speed boost description", Icon = "SpeedUp.png", Effect = Skill.SkillEffect.SpeedBoost });
        Skills.Add(new Skill { ID = 6, Name = "Health Recovery", Description = "Health recovery description", Icon = "Heal.png", Effect = Skill.SkillEffect.Heal });
        Skills.Add(new Skill { ID = 7, Name = "Max Health Boost", Description = "Max health boost description", Icon = "HpUp.png", Effect = Skill.SkillEffect.MaxHealthBoost });
        Skills.Add(new Skill { ID = 8, Name = "Attack Speed Boost", Description = "Attack speed boost description", Icon = "ShotSpeedUp.png", Effect = Skill.SkillEffect.AttackSpeedBoost });
    }
}
