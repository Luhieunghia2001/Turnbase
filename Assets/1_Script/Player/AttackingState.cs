using System.Collections;
using UnityEngine;



public class AttackingState : BaseState
{
    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang tấn công.");
        // Bắt đầu animation tấn công
        stateMachine.StartCoroutine(FinishAttack());
    }

    private IEnumerator FinishAttack()
    {
        yield return new WaitForSeconds(1.0f); // Thời gian của animation
        // Gây sát thương
        // Kiểm tra xem có mục tiêu không
        if (stateMachine.character.target != null)
        {
            Debug.Log(stateMachine.character.gameObject.name + " tấn công " + stateMachine.character.target.gameObject.name);
            // Gây sát thương lên mục tiêu
            // stateMachine.character.target.stats.TakeDamage(damage);
        }

        // Sau khi hoàn thành, chuyển lại về trạng thái chờ
        stateMachine.SwitchState(stateMachine.waitingState);
    }
}