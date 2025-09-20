using System.Collections;
using UnityEngine;

public class AttackingState : BaseState
{
    private Character target;


    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        target = stateMachine.character.target;

        if (target == null || !target.isAlive)
        {
            Debug.LogWarning("Mục tiêu không hợp lệ hoặc đã chết. Trở lại trạng thái chờ.");
            stateMachine.battleManager.EndTurn(stateMachine.character);
            return;
        }

        // Tạo skill cơ bản (basic attack)
        Skill basicAttack = ScriptableObject.CreateInstance<Skill>();
        basicAttack.skillName = "Basic Attack";
        basicAttack.damage = stateMachine.character.stats.attack;
        basicAttack.skillType = SkillType.Damage;

        ICommand command = new AttackCommand(stateMachine.character, target, basicAttack);
        stateMachine.character.StartCoroutine(ExecuteCommand(command));

        //PlayerActionUI.Instance.Hide();


        


    }

    private IEnumerator ExecuteCommand(ICommand command)
    {
        yield return stateMachine.character.StartCoroutine(command.Execute());
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnUpdate() { }

    public override void OnExit()
    {

        stateMachine.character.StopAllCoroutines();
    }
}
