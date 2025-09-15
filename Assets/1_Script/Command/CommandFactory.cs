using UnityEngine;

public static class SkillCommandFactory
{
    public static ICommand CreateCommand(Character user, Character target, Skill skill, BattleManager battleManager)
    {
        switch (skill.skillType)
        {
            case SkillType.Damage:
                return new AttackCommand(user, target, skill);
            case SkillType.Heal:
                return new HealCommand(user, target, skill);
            case SkillType.Buff:
                return new HealCommand(user, target, skill);
            case SkillType.Special:
                return new HealCommand(user, target, skill);
            case SkillType.DamageAll:
                return new DamageAllCommand(user, skill, battleManager);
            default:
                Debug.LogWarning("Skill chưa được hỗ trợ: " + skill.skillType);
                return null;
        }
    }
}
