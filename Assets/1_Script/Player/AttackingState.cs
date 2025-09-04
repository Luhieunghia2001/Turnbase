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

        if (target != null && target.isAlive)
        {
            stateMachine.character.StartCoroutine(MoveAndAttack());
        }
        else
        {
            Debug.LogWarning("Mục tiêu không hợp lệ hoặc đã chết. Trở lại trạng thái chờ.");
            // Kết thúc lượt ngay lập tức nếu không có mục tiêu
            stateMachine.battleManager.EndTurn(stateMachine.character);
        }
    }

    private IEnumerator MoveAndAttack()
    {
        // 1. Di chuyển đến mục tiêu
        Vector3 initialPosition = stateMachine.character.initialPosition;
        float attackDistance = 1.5f; // Khoảng cách tấn công tùy chỉnh

        // Tính toán vị trí đích dựa trên vị trí của mục tiêu và người tấn công
        float direction = Mathf.Sign(target.transform.position.x - stateMachine.character.transform.position.x);
        Vector3 destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

        // Chạy animation
        stateMachine.character.animator.Play("Run");
        stateMachine.character.animator.SetBool("IsRunning", true);

        while (Vector3.Distance(stateMachine.character.transform.position, destination) > 0.1f)
        {
            stateMachine.character.transform.position = Vector3.MoveTowards(
                stateMachine.character.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 2. Tấn công và chờ đợi
        stateMachine.character.animator.SetBool("IsRunning", false);
        stateMachine.character.animator.SetTrigger("Attack");

        // Gây sát thương
        yield return new WaitForSeconds(1f); // Chờ một chút để khớp với animation
        target.TakeDamage(stateMachine.character.stats.attack);
        yield return new WaitForSeconds(1.5f); // Chờ phần còn lại của animation

        // 3. Quay trở về vị trí ban đầu
        stateMachine.character.animator.SetBool("IsRunning", true);

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

        // 4. Kết thúc lượt
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnUpdate() { }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
