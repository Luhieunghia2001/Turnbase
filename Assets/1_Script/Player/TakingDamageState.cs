using UnityEngine;
using System.Collections;

public class TakingDamageState : BaseState
{
    public TakingDamageState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang bị nhận sát thương.");
        stateMachine.character.StartCoroutine(HandleTakingDamage());
    }

    private IEnumerator HandleTakingDamage()
    {
        // Kích hoạt hoạt ảnh bị đánh
        stateMachine.character.animator.Play("Idle_Hurt");

        // Chờ một khoảng thời gian ngắn để hoạt ảnh hoàn tất
        // Thời gian chờ có thể điều chỉnh để phù hợp với độ dài hoạt ảnh
        yield return new WaitForSeconds(0.5f);

        // Sau khi hoạt ảnh kết thúc, kiểm tra xem nhân vật còn sống hay không
        if (stateMachine.character.isAlive)
        {
            // Nếu còn sống, chuyển về trạng thái chờ
            stateMachine.SwitchState(stateMachine.waitingState);
        }
        else
        {
            // Nếu chết, chuyển sang trạng thái chết
            stateMachine.SwitchState(stateMachine.deadState);
        }
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
