using UnityEngine;
using System.Collections;

public class AttackingState : BaseState
{
    private Character target;
    private float moveSpeed = 5f;

    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang tấn công.");
        target = stateMachine.character.target;

        if (target != null)
        {
            stateMachine.character.StartCoroutine(MoveAndAttack());
        }
        else
        {
            Debug.LogWarning("Không có mục tiêu nào được chọn. Trở lại trạng thái chờ.");
            stateMachine.SwitchState(stateMachine.waitingState);
        }
    }

    private IEnumerator MoveAndAttack()
    {
        Debug.Log("Mục tiêu đã được chọn: " + target.gameObject.name);

        Vector3 initialPosition = stateMachine.character.initialPosition;
        Vector3 destination;

        if (stateMachine.character.isPlayer)
        {
            destination = target.transform.position;
            destination.x += 1f; // Khoảng cách tấn công cho Player
        }
        else
        {
            destination = target.transform.position;
            destination.x -= 1f; // Khoảng cách tấn công cho Enemy
        }
        stateMachine.character.animator.Play("Run");

        stateMachine.character.animator.SetBool("IsRunning", true);

        // Di chuyển đến vị trí mục tiêu
        while (Vector3.Distance(stateMachine.character.transform.position, destination) > 0.1f)
        {
            stateMachine.character.transform.position = Vector3.MoveTowards(
                stateMachine.character.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log(stateMachine.gameObject.name + " đã đến gần " + target.gameObject.name);

        // Sau khi di chuyển đến nơi, bắt đầu animation tấn công
        stateMachine.character.animator.SetBool("IsRunning", false);
        stateMachine.character.animator.SetTrigger("Attack");

        // Chờ một chút để đồng bộ với hoạt ảnh
        yield return new WaitForSeconds(1.5f);

        // Thực hiện tấn công
        Debug.Log(stateMachine.gameObject.name + " tấn công " + target.gameObject.name);

        // Bắt đầu animation chạy ngược về
        stateMachine.character.animator.SetBool("IsRunning", true);

        // Quay về vị trí ban đầu
        while (Vector3.Distance(stateMachine.character.transform.position, initialPosition) > 0.1f)
        {
            stateMachine.character.transform.position = Vector3.MoveTowards(
                stateMachine.character.transform.position,
                initialPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        stateMachine.character.animator.SetBool("IsRunning", false);

        // Kết thúc lượt của nhân vật hiện tại
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnUpdate()
    {
        // Không cần logic gì ở đây vì tất cả được xử lý trong Coroutine
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
