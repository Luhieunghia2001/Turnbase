using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAllCommand : ICommand
{
    private Character user;
    private Skill skill;
    private BattleManager battleManager;

    public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
    {
        this.user = user;
        this.skill = skill;
        this.battleManager = battleManager;
    }

    public IEnumerator Execute()
    {
        Debug.Log($"{user.name} dùng skill AOE {skill.skillName}");

        user.animator.Play("Cast");
        yield return new WaitForSeconds(1.5f);

        List<Character> allTargets;
        if (skill.targetType == SkillTargetType.Enemies)
        {
            allTargets = battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
        }
        else // Allies
        {
            allTargets = battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
        }

        foreach (Character aoeTarget in allTargets)
        {
            aoeTarget.TakeDamage(skill.damage);
        }

        yield return new WaitForSeconds(0.5f);
    }
}
