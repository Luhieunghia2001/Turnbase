using UnityEngine;
using System.Collections;

public class AttackingState : BaseState
{
    private Character target;
    private float moveSpeed = 5f; // Đã thêm biến này vào đây để tránh lỗi biên dịch

    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }


    
    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang tấn công.");
        target = stateMachine.character.target;

        // Bắt đầu coroutine để xử lý di chuyển và tấn công
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
        // Debug để kiểm tra mục tiêu có đúng không
        Debug.Log("Mục tiêu đã được chọn: " + target.gameObject.name + " tại vị trí slot: " + stateMachine.battleManager.allCombatants.IndexOf(target));

        Vector3 initialPosition = stateMachine.character.initialPosition;
        Vector3 targetPosition = target.transform.position;

        // Xác định điểm đến (destination)
        Vector3 destination = target.transform.position;

        // Điều chỉnh vị trí tấn công tùy thuộc vào loại nhân vật (Player hay Enemy)
        if (stateMachine.character.isPlayer) // Giả sử bạn có một biến IsPlayer trong class Character
        {
            // Nếu là Player, di chuyển đến trước mặt Enemy
            destination.x += 1.5f; // Đứng bên phải Enemy
        }
        else
        {
            // Nếu là Enemy, di chuyển đến trước mặt Player
            destination.x -= 1.5f; // Đứng bên trái Player
        }

        stateMachine.character.animator.Play("Run");

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

        stateMachine.character.animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1.5f); // Chờ một chút để đồng bộ với hoạt ảnh
        // Thực hiện tấn công
        Debug.Log(stateMachine.gameObject.name + " tấn công " + target.gameObject.name + " tại vị trí slot: " + stateMachine.battleManager.allCombatants.IndexOf(target));

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

        // Sau khi quay về, chuyển animation về trạng thái chờ (Idle)
        stateMachine.character.animator.SetBool("IsRunning", false);


        // Kết thúc lượt của nhân vật hiện tại
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnExit()
    {
        // Có thể thêm logic dọn dẹp ở đây nếu cần
    }
}
