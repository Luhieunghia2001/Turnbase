using System.Collections;
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
        if (target != null && target.isAlive)
        {
            stateMachine.character.StartCoroutine(MoveAndCastSkill());
        }
        else
        {
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
            // ... các loại khác
        }
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    private IEnumerator DamageSkillRoutine()
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
        target.TakeDamage(selectedSkill.damage);

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
    }

    private IEnumerator HealSkillRoutine()
    {
        Character playerTarget = stateMachine.battleManager.allCombatants.FirstOrDefault(c => c.isPlayer);

        Debug.LogWarning("Healing");

        yield return null;
    }

    private IEnumerator BuffSkillRoutine()
    {
        // Animation, tăng chỉ số, hiệu ứng...
        yield return null;
    }

    private IEnumerator SpecialSkillRoutine()
    {
        // Animation, hiệu ứng đặc biệt...
        yield return null;
    }
}
