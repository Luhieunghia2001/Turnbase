using UnityEngine;
using UnityEngine.UI;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class PlayerActionUI : MonoBehaviour
{
    public BattleManager battleManager;

    public Button attackButton;
    public Button skillButton;
    public Button parryButton;
    public Button confirmButton;

    [Header("Skill Buttons")]
    public List<Button> skillButtons;


    public GameObject playerActionsPanel;
    public GameObject PlayerSkill;

    // Thêm tham chiếu đến Slider để quản lý thanh parry
    public Slider parrySlider;

    // Delegate để thông báo cho BattleManager biết khi nào nút Parry được nhấn
    public Action OnParryAttempted;

    private Skill selectedSkillToConfirm;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackClicked);
        skillButton.onClick.AddListener(OnSkillClicked);
        parryButton.onClick.AddListener(OnParryClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);


        PlayerSkill.gameObject.SetActive(false);

    }

    public void Show()
    {
        playerActionsPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        // Đảm bảo các UI liên quan đến parry bị ẩn
        if (parryButton != null) parryButton.gameObject.SetActive(false);
        if (parrySlider != null) parrySlider.gameObject.SetActive(false);
    }

    public void Hide()
    {
        playerActionsPanel.SetActive(false);
    }

    // Phương thức mới để hiển thị thanh slider và nút parry
    public void ShowParryUI(bool showParry)
    {
        if (parrySlider != null)
        {
            parrySlider.gameObject.SetActive(showParry);
        }
        if (parryButton != null)
        {
            parryButton.gameObject.SetActive(showParry);
        }
    }

    private void OnAttackClicked()
    {
        Debug.Log("OnAttackClicked được gọi.");
        if (battleManager.activeCharacter != null && battleManager.activeCharacter.isPlayer)
        {
            // Chuyển sang ReadyState để người chơi chọn mục tiêu
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.readyState);
        }
        // Hiển thị nút xác nhận sau khi chọn hành động
        confirmButton.gameObject.SetActive(true);
    }

    private void OnSkillClicked()
    {
        Debug.Log("Sử dụng Kỹ năng!");
        PlayerSkill.SetActive(true);
    }

    private void OnSkillButtonClicked(Skill selectedSkill)
    {
        Debug.Log($"OnSkillButtonClicked được gọi với kỹ năng: {selectedSkill.skillName}");
        // Chuyển sang ReadyStateSkill để người chơi chọn mục tiêu
        battleManager.activeCharacter.stateMachine.SwitchState(
            new ReadyStateSkill(battleManager.activeCharacter.stateMachine, selectedSkill)
        );

        // Lưu lại kỹ năng đã chọn vào biến tạm thời
        selectedSkillToConfirm = selectedSkill;

        // Chỉ hiển thị nút xác nhận, không ẩn skillButtons
        confirmButton.gameObject.SetActive(true);
    }

    public void SetupSkillUI(List<Skill> skills)
    {
        Debug.Log("SetupSkillUI được gọi.");

        for (int i = 0; i < skills.Count && i < skillButtons.Count; i++)
        {
            Button currentButton = skillButtons[i];

            currentButton.onClick.RemoveAllListeners();

            Skill skillToUse = skills[i];

            currentButton.onClick.AddListener(() => OnSkillButtonClicked(skillToUse));

            Image buttonImage = currentButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = currentButton.GetComponentInChildren<Image>();
            }

            if (buttonImage != null && skillToUse.icon != null)
            {
                buttonImage.sprite = skillToUse.icon;
                buttonImage.color = Color.white;
            }
            else if (buttonImage != null)
            {
                buttonImage.sprite = null;
                buttonImage.color = new Color(1, 1, 1, 0);
            }

            currentButton.gameObject.SetActive(true);
        }
    }

    private void OnParryClicked()
    {
        if (OnParryAttempted != null)
        {
            OnParryAttempted();
        }
    }

    private void OnConfirmClicked()
    {
        Debug.Log("OnConfirmClicked được gọi.");

        // Kiểm tra xem nhân vật có đang ở trạng thái ReadyStateSkill không
        if (battleManager.activeCharacter.stateMachine.currentState is ReadyStateSkill currentState)
        {
            Debug.Log("Gọi OnConfirm() của ReadyStateSkill.");
            // Gọi phương thức OnConfirm của trạng thái hiện tại (ReadyStateSkill)
            currentState.OnConfirm();

            // Ẩn các UI không cần thiết
            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false);
        }
        else if (battleManager.activeCharacter.stateMachine.currentState is ReadyState)
        {
            Debug.Log("Chuyển từ ReadyState sang AttackingState.");
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.attackingState);
            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false);
        }
    }
}
