using UnityEngine;
using System.Collections;

public class ParryingState : BaseState
{
    public ParryingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang thực hiện đỡ đòn.");
        stateMachine.character.StartCoroutine(HandleParry());
    }

    private IEnumerator HandleParry()
    {
        // Kích hoạt hoạt ảnh parry
        stateMachine.character.animator.SetTrigger("Parry");

        // Chờ một khoảng thời gian ngắn để hoạt ảnh hoàn tất
        yield return new WaitForSeconds(0.8f);

        // Sau khi hoạt ảnh kết thúc, chuyển về trạng thái chờ
        stateMachine.SwitchState(stateMachine.waitingState);
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
