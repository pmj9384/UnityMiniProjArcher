[System.Serializable]
public class Skill
{
    public int ID;
    public string Name;
    public string Description;
    public string Icon;  // 아이콘 경로
    public SkillEffect Effect;  // 스킬 효과

    // 능력에 대한 정의
    public enum SkillEffect
    {
        DiagonalArrow,   // 사선 화살
        FireArrow,       // 화염 화살
        FrostArrow,      // 얼음 화살
        AttackBoost,     // 공격력 증가
        SpeedBoost,      // 이동 속도 증가
        Heal,            // 체력 회복
        MaxHealthBoost,  // 최대 체력 증가
        AttackSpeedBoost // 공격 속도 증가
    }
}
