using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ReadyStateSkill : BaseState
{
    private Skill selectedSkill; // Thêm dòng này để khai báo biến

    private List<Character> possibleTargets;
    private int currentIndex;

    // Chỉnh sửa constructor để nhận tham số Skill
    public ReadyStateSkill(CharacterStateMachine stateMachine, Skill skill) : base(stateMachine)
    {
        this.selectedSkill = skill; // Gán skill được truyền vào
    }

    public override void OnEnter()
    {
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log($"Entering ReadyStateSkill with skill: {selectedSkill.skillName}");

        ShowTargetMarker(false);
        stateMachine.character.target = null;

        // Tùy thuộc vào loại mục tiêu của kỹ năng, lọc danh sách mục tiêu
        switch (selectedSkill.targetType)
        {
            case SkillTargetType.Self:
                possibleTargets = new List<Character> { stateMachine.character };
                break;
            case SkillTargetType.Ally:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c.isPlayer);
                break;
            case SkillTargetType.Enemy:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);
                break;
            case SkillTargetType.Allies:
                // Nếu là AoE cho đồng minh, bạn có thể chọn một mục tiêu đầu tiên
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c.isPlayer);
                break;
            case SkillTargetType.AllEnemie:
                // Nếu là AoE cho kẻ địch, bạn cũng có thể chọn một mục tiêu đầu tiên
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);
                break;
        }

        if (possibleTargets.Count > 0)
        {
            currentIndex = 0;
            stateMachine.character.target = possibleTargets[currentIndex];

            if (stateMachine.character.isPlayer)
            {
                if (stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }
            }
        }
        else
        {
            stateMachine.character.target = null;
            Debug.LogWarning("Không tìm thấy mục tiêu khả dụng cho kỹ năng này.");
        }
    }
    public override void OnUpdate()
    {
        if (stateMachine.character.isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.A))
                UpdateTarget(-1);
            if (Input.GetKeyDown(KeyCode.D))
                UpdateTarget(1);
        }
    }

    private void UpdateTarget(int direction)
    {
        if (possibleTargets.Count > 0)
        {
            // Ẩn marker của mục tiêu hiện tại
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(false);
            }

            currentIndex = (currentIndex + direction + possibleTargets.Count) % possibleTargets.Count;
            stateMachine.character.target = possibleTargets[currentIndex];

            Debug.Log("Đã chuyển mục tiêu sang: " + stateMachine.character.target.gameObject.name + " tại vị trí slot: " + currentIndex);

            // Hiển thị marker cho mục tiêu mới, bất kể đó là ai
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(true);
            }
        }
    }

    public override void OnExit()
    {
        ShowTargetMarker(false);
    }

    private void ShowTargetMarker(bool active)
    {
        if (possibleTargets != null)
        {
            foreach (Character target in possibleTargets)
            {
                if (target != null && target.targetMarker != null)
                    target.targetMarker.SetActive(active);
            }
        }
    }

    // Thêm phương thức OnConfirm() để chuyển trạng thái tấn công
    public void OnConfirm()
    {
        stateMachine.SwitchState(new SkillAttackingState(stateMachine, selectedSkill));
    }
}