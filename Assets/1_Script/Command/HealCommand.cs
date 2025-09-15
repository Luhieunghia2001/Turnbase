using System.Collections;
using UnityEngine;

public class HealCommand : SkillCommand
{
    public HealCommand(Character user, Character target, Skill skill)
        : base(user, target, skill) { }

    public override IEnumerator Execute()
    {
        Debug.Log($"{user.name} dùng skill hồi máu!");
        user.animator.Play("Buff");
        yield return new WaitForSeconds(1.5f);

        target.Heal(skill.damage); 
        yield return new WaitForSeconds(0.5f);
    }
}
