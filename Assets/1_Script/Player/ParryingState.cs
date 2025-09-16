using UnityEngine;

public class ParryingState : BaseState
{
    public ParryingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log($"{stateMachine.character.name} chuyển sang ParryingState.");

        var cmd = new ParryCommand(stateMachine.character);
        stateMachine.character.battleManager.EnqueueCommand(cmd);
    }
}

