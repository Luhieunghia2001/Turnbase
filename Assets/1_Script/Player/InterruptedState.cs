using UnityEngine;
using System.Collections;

public class InterruptedState : BaseState
{
    private float moveSpeed = 5f;

    public InterruptedState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đã bị gián đoạn và đang nhận sát thương.");
        stateMachine.character.StartCoroutine(HandleInterruption());
    }

    private IEnumerator HandleInterruption()
    {
        // 1. Kích hoạt hoạt ảnh bị đánh (hit/damage animation)
        stateMachine.character.animator.SetTrigger("Hit");
        stateMachine.character.animator.SetTrigger("GetUp");
        stateMachine.character.animator.SetBool("IsIdle", false);



        // Chờ một khoảng thời gian để hoạt ảnh hoàn tất
        yield return new WaitForSeconds(5);

        stateMachine.character.animator.SetBool("IsRunningOut", true);




        // 2. Quay về vị trí ban đầu
        Vector3 initialPosition = stateMachine.character.initialPosition;

        while (Vector3.Distance(stateMachine.character.transform.position, initialPosition) > 0.1f)
        {
            stateMachine.character.transform.position = Vector3.MoveTowards(
                stateMachine.character.transform.position,
                initialPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }


        // 3. Sau khi quay về, kết thúc lượt của nhân vật bị gián đoạn
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log(stateMachine.gameObject.name + " đã quay về vị trí ban đầu sau khi bị gián đoạn.");

        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
