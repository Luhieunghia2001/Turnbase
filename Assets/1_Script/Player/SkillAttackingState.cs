using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillAttackingState : BaseState
{
    private Character target;
    private Skill selectedSkill;
    private float moveSpeed = 5f;

    public SkillAttackingState(CharacterStateMachine stateMachine, Skill skill) : base(stateMachine)
    {
        selectedSkill = skill;
    }

    public override void OnEnter()
    {
        target = stateMachine.character.target;
        if (selectedSkill.targetType == SkillTargetType.Enemies || selectedSkill.targetType == SkillTargetType.Allies)
        {
            // Đối với kỹ năng AOE, không cần kiểm tra mục tiêu đơn lẻ
            stateMachine.character.StartCoroutine(MoveAndCastSkill());
        }
        else if (target != null && target.isAlive)
        {
            stateMachine.character.StartCoroutine(MoveAndCastSkill());
        }
        else
        {
            // Nếu không có mục tiêu hợp lệ, kết thúc lượt ngay lập tức
            Debug.Log("Không có mục tiêu hợp lệ, kết thúc lượt.");
            stateMachine.battleManager.EndTurn(stateMachine.character);
        }
    }

    private IEnumerator MoveAndCastSkill()
    {
        switch (selectedSkill.skillType)
        {
            case SkillType.Damage:
                yield return DamageSkillRoutine();
                break;
            case SkillType.Heal:
                yield return HealSkillRoutine();
                break;
            case SkillType.Buff:
                yield return BuffSkillRoutine();
                break;
            case SkillType.Special:
                yield return SpecialSkillRoutine();
                break;
            case SkillType.DamageAll:
                yield return DamageAllSkillRoutine();
                break;
        }
        // Kết thúc lượt sau khi thực hiện xong kỹ năng
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    private IEnumerator DamageSkillRoutine()
    {
        // 1. Di chuyển đến mục tiêu
        Vector3 initialPosition = stateMachine.character.initialPosition;
        float attackDistance = 1.5f; // Khoảng cách tấn công tùy chỉnh

        float direction = Mathf.Sign(target.transform.position.x - stateMachine.character.transform.position.x);
        Vector3 destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

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

        yield return new WaitForSeconds(1f);

        // Gây sát thương cho một mục tiêu duy nhất
        target.TakeDamage(selectedSkill.damage);

        yield return new WaitForSeconds(1.5f);

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
    }

    private IEnumerator DamageAllSkillRoutine()
    {
        Debug.Log("Đang thực hiện kỹ năng tấn công diện rộng.");

        // Chạy animation "Cast"
        stateMachine.character.animator.Play("Cast");

        // Chờ animation hoàn thành
        yield return new WaitForSeconds(1.5f);

        List<Character> allTargets;
        if (selectedSkill.targetType == SkillTargetType.Enemies)
        {
            allTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
        }
        else // SkillTargetType.Allies
        {
            allTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
        }

        foreach (Character aoeTarget in allTargets)
        {
            aoeTarget.TakeDamage(selectedSkill.damage);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator HealSkillRoutine()
    {
        Debug.Log("Đang thực hiện kỹ năng hồi máu.");

        stateMachine.character.animator.Play("Buff");


        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator BuffSkillRoutine()
    {
        Debug.Log("Đang thực hiện kỹ năng buff.");

        stateMachine.character.animator.Play("Buff");


        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator SpecialSkillRoutine()
    {
        Debug.Log("Đang thực hiện kỹ năng đặc biệt.");
        // Chạy animation "Special"
        stateMachine.character.animator.SetTrigger("Special");
        yield return new WaitForSeconds(1.5f);

        // Logic cho kỹ năng đặc biệt
        // Ví dụ: gây sát thương đặc biệt, thay đổi trạng thái game...

        yield return new WaitForSeconds(0.5f);
    }
}
