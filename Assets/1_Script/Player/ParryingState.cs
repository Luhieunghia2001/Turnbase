using UnityEngine;
using System.Collections;

public class ParryingState : BaseState
{
    public ParryingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log($"{stateMachine.character.name} chuyển sang ParryingState.");

        var cmd = new ParryCommand(stateMachine.character);
        stateMachine.character.StartCoroutine(ExecuteCommand(cmd));
    }

    private IEnumerator ExecuteCommand(ICommand command)
    {
        yield return stateMachine.character.StartCoroutine(command.Execute());
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
