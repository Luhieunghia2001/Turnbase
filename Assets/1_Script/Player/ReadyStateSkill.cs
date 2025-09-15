using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ReadyStateSkill : BaseState
{
    private Skill selectedSkill;
    private List<Character> possibleTargets;
    private int currentIndex;

    public ReadyStateSkill(CharacterStateMachine stateMachine, Skill skill) : base(stateMachine)
    {
        this.selectedSkill = skill;
    }

    public override void OnEnter()
    {
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log($"Entering ReadyStateSkill with skill: {selectedSkill.skillName}");

        ShowTargetMarker(false);
        stateMachine.character.target = null;

        // Lọc danh sách mục tiêu dựa trên loại mục tiêu của kỹ năng, thêm kiểm tra null
        switch (selectedSkill.targetType)
        {
            case SkillTargetType.Self:
                possibleTargets = new List<Character> { stateMachine.character };
                break;
            case SkillTargetType.Ally:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
                break;
            case SkillTargetType.Enemy:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
                break;
            case SkillTargetType.Allies:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
                break;
            case SkillTargetType.Enemies:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
                break;
        }

        if (possibleTargets.Count > 0)
        {
            if (selectedSkill.targetType == SkillTargetType.Enemies || selectedSkill.targetType == SkillTargetType.Allies)
            {
                foreach (Character character in possibleTargets)
                {
                    if (character != null)
                    {
                        if (character.targetMarker != null)
                        {
                            character.targetMarker.SetActive(true);
                        }
                        else
                        {
                            Debug.LogError($"Lỗi: Target marker bị thiếu trên nhân vật: {character.gameObject.name}. Vui lòng gán trong Inspector.");
                        }
                    }
                }
                stateMachine.character.target = null;
            }
            else
            {
                currentIndex = 0;
                stateMachine.character.target = possibleTargets[currentIndex];

                if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }
                else
                {
                    Debug.LogError($"Lỗi: Target marker bị thiếu trên nhân vật: {stateMachine.character.target.gameObject.name}. Vui lòng gán trong Inspector.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy mục tiêu khả dụng cho kỹ năng: {selectedSkill.skillName}. Tự động hủy.");
            OnCancel();
        }
    }

    public override void OnUpdate()
    {
        if (stateMachine.character.isPlayer)
        {
            if (selectedSkill.targetType == SkillTargetType.Enemy || selectedSkill.targetType == SkillTargetType.Ally)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    UpdateTarget(-1);
                if (Input.GetKeyDown(KeyCode.D))
                    UpdateTarget(1);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                OnCancel();
        }
    }

    private void UpdateTarget(int direction)
    {
        if (possibleTargets.Count > 0)
        {
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(false);
            }

            currentIndex = (currentIndex + direction + possibleTargets.Count) % possibleTargets.Count;
            stateMachine.character.target = possibleTargets[currentIndex];

            Debug.Log("Đã chuyển mục tiêu sang: " + stateMachine.character.target.gameObject.name + " tại vị trí slot: " + currentIndex);

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
                {
                    target.targetMarker.SetActive(active);
                }
            }
        }
    }

    public void OnConfirm()
    {
        if (selectedSkill.targetType == SkillTargetType.Enemies || selectedSkill.targetType == SkillTargetType.Allies)
        {
            stateMachine.character.target = possibleTargets[0];
        }

        stateMachine.SwitchState(new SkillAttackingState(stateMachine, selectedSkill));
    }

    public void OnCancel()
    {
        if (stateMachine.character.ownUI != null)
        {
            stateMachine.character.ownUI.PlayerSkill.SetActive(false);
            stateMachine.character.ownUI.confirmButton.gameObject.SetActive(false);
        }

        stateMachine.character.target = null;
        stateMachine.SwitchState(stateMachine.readyState);
    }

}
