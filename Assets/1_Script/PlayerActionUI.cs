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


        // Ẩn tất cả các nút ban đầu để chuẩn bị cập nhật
        //for (int i = 0; i < skillButtons.Count; i++)
        //{
        //    skillButtons[i].gameObject.SetActive(false);
        //}

        // Lặp qua danh sách kỹ năng và cập nhật nút tương ứng
        for (int i = 0; i < skills.Count && i < skillButtons.Count; i++)
        {
            Button currentButton = skillButtons[i];

            // Xóa tất cả các listener cũ
            currentButton.onClick.RemoveAllListeners();

            // Lấy Skill ScriptableObject
            Skill skillToUse = skills[i];

            // Gán sự kiện và truyền Skill vào
            currentButton.onClick.AddListener(() => OnSkillButtonClicked(skillToUse));

            // --- CHỈ CẬP NHẬT HÌNH ẢNH Ở ĐÂY ---
            Image buttonImage = currentButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                // Tìm Image trong đối tượng con nếu nó không nằm trên Button
                buttonImage = currentButton.GetComponentInChildren<Image>();
            }

            if (buttonImage != null && skillToUse.icon != null)
            {
                buttonImage.sprite = skillToUse.icon;
                buttonImage.color = Color.white; // Đặt màu trắng để icon hiện rõ
            }
            else if (buttonImage != null)
            {
                // Nếu không có icon, ẩn hình ảnh đi
                buttonImage.sprite = null;
                buttonImage.color = new Color(1, 1, 1, 0); // Màu trong suốt
            }
            // --- KẾT THÚC CẬP NHẬT HÌNH ẢNH ---

            // Hiển thị nút
            currentButton.gameObject.SetActive(true);
        }
    }


    private void OnParryClicked()
    {
        // Thông báo cho BattleManager rằng người chơi muốn parry
        if (OnParryAttempted != null)
        {
            OnParryAttempted();
        }
    }

    private void OnConfirmClicked()
    {
        // Kiểm tra xem nhân vật có đang ở trạng thái ReadyStateSkill không
        if (battleManager.activeCharacter.stateMachine.currentState is ReadyStateSkill)
        {
            // Chuyển sang trạng thái tấn công với kỹ năng đã được lưu
            battleManager.activeCharacter.stateMachine.SwitchState(
                new SkillAttackingState(battleManager.activeCharacter.stateMachine, selectedSkillToConfirm)
            );

            // Ẩn cả PlayerSkill và confirmButton
            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false);
        }
        // Logic cho trạng thái ReadyState ban đầu vẫn giữ nguyên
        else if (battleManager.activeCharacter.stateMachine.currentState is ReadyState)
        {
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.attackingState);
            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false); // Đảm bảo các nút skill cũng bị ẩn
        }
    }
}
