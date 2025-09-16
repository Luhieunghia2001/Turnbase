using System.Collections;
using UnityEngine;

public class ParryCommand : ICommand
{
    private Character character;

    public ParryCommand(Character character)
    {
        this.character = character;
    }

    public IEnumerator Execute()
    {
        Debug.Log($"{character.name} bắt đầu Parry!");

        // Gọi animation
        character.animator.SetTrigger("Parry");

        // Thời gian "cửa sổ parry" hoặc chờ animation
        yield return new WaitForSeconds(0.8f);

        // Kết thúc parry, quay về WaitingState
        character.stateMachine.SwitchState(character.stateMachine.waitingState);

        Debug.Log($"{character.name} kết thúc Parry.");
    }
}
