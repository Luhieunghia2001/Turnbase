using System.Collections;
using UnityEngine;

public interface ICommand
{
    IEnumerator Execute();
}


public abstract class SkillCommand : ICommand
{
    protected Character user;
    protected Character target;
    protected Skill skill;

    public SkillCommand(Character user, Character target, Skill skill)
    {
        this.user = user;
        this.target = target;
        this.skill = skill;
    }

    public abstract IEnumerator Execute();
}