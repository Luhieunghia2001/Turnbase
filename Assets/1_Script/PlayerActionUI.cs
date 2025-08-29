using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerActionUI : MonoBehaviour
{
    public BattleManager battleManager;

    public Button attackButton;
    public Button skillButton;
    public Button parryButton;
    public Button confirmButton;

    public GameObject playerActionsPanel;

    // Thêm tham chiếu đến Slider để quản lý thanh parry
    public Slider parrySlider;

    // Delegate để thông báo cho BattleManager biết khi nào nút Parry được nhấn
    public Action OnParryAttempted;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackClicked);
        skillButton.onClick.AddListener(OnSkillClicked);
        parryButton.onClick.AddListener(OnParryClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);
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
        // Khi người chơi xác nhận, chuyển sang trạng thái tấn công
        if (battleManager.activeCharacter != null && battleManager.activeCharacter.isPlayer)
        {
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.attackingState);
        }
    }
}
