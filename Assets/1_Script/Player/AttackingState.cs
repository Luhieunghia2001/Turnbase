using System.Collections;
using UnityEngine;

public class AttackingState : BaseState
{
    // Tên của trigger animation trong Animator Controller
    private const string ATTACK_TRIGGER = "Attack";

    // Tốc độ di chuyển của nhân vật
    private float moveSpeed = 5.0f;

    // Khoảng cách tối thiểu để bắt đầu tấn công
    private float attackRange = 4.0f;

    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang tấn công.");

        // Bắt đầu coroutine để xử lý việc di chuyển và tấn công
        stateMachine.StartCoroutine(MoveAndAttack());
    }

    private IEnumerator MoveAndAttack()
    {
        // Lưu vị trí ban đầu của nhân vật
        Vector3 startPosition = stateMachine.transform.position;

        // Kiểm tra xem có mục tiêu không
        if (stateMachine.character.target == null)
        {
            Debug.Log("Không có mục tiêu để tấn công. Chuyển về trạng thái chờ.");
            stateMachine.SwitchState(stateMachine.waitingState);
            yield break; // Kết thúc coroutine
        }

        // --- LOG ĐỂ KIỂM TRA MỤC TIÊU ĐÃ CHỌN ---
        var enemies = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);
        int targetIndex = enemies.IndexOf(stateMachine.character.target);
        Debug.Log("Mục tiêu đã được chọn: " + stateMachine.character.target.gameObject.name + " tại vị trí slot: " + targetIndex);

        // Vòng lặp di chuyển đến mục tiêu
        while (Vector3.Distance(stateMachine.transform.position, stateMachine.character.target.transform.position) > attackRange)
        {
            // Di chuyển về phía mục tiêu
            Vector3 targetPosition = stateMachine.character.target.transform.position;
            Vector3 direction = (targetPosition - stateMachine.transform.position).normalized;
            stateMachine.transform.position += direction * moveSpeed * Time.deltaTime;

            // Quay mặt về phía mục tiêu
            stateMachine.transform.LookAt(stateMachine.character.target.transform);

            yield return null; // Chờ frame tiếp theo
        }

        // Đã đến gần mục tiêu, bây giờ thực hiện tấn công
        Debug.Log(stateMachine.character.gameObject.name + " đã đến gần " + stateMachine.character.target.gameObject.name);

        // Gán trigger để gọi animation tấn công
        if (stateMachine.character.animator != null)
        {
            stateMachine.character.animator.SetTrigger(ATTACK_TRIGGER);
        }

        // Chờ animation tấn công kết thúc
        yield return new WaitForSeconds(1.0f); // Thời gian của animation

        // Gây sát thương
        Debug.Log(stateMachine.character.gameObject.name + " tấn công " + stateMachine.character.target.gameObject.name + " tại vị trí slot: " + targetIndex);
        // Gây sát thương lên mục tiêu
        // stateMachine.character.target.stats.TakeDamage(damage);

        // Quay trở về vị trí ban đầu
        while (Vector3.Distance(stateMachine.transform.position, startPosition) > 0.1f)
        {
            stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Sau khi hoàn thành, chuyển lại về trạng thái chờ
        stateMachine.SwitchState(stateMachine.waitingState);
    }

    public override void OnUpdate()
    {
        // Không làm gì ở đây vì logic được xử lý trong coroutine
    }
}
