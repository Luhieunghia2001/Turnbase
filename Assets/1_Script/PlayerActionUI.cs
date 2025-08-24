using UnityEngine;
using UnityEngine.UI;

// Script này quản lý các hành động của người chơi thông qua UI
public class PlayerActionUI : MonoBehaviour
{
    // Tham chiếu đến BattleManager để điều phối hành động
    public BattleManager battleManager;

    // Các nút bấm để người chơi tương tác
    public Button attackButton;
    public Button skillButton;
    public Button parryButton;

    // GameObject chứa UI để ẩn/hiện
    public GameObject playerActionsPanel;

    private void Start()
    {
        // Thêm listener cho các nút
        attackButton.onClick.AddListener(OnAttackClicked);
        skillButton.onClick.AddListener(OnSkillClicked);
        parryButton.onClick.AddListener(OnParryClicked);
    }

    // Hiển thị panel UI
    public void Show()
    {
        playerActionsPanel.SetActive(true);
    }

    // Ẩn panel UI
    public void Hide()
    {
        playerActionsPanel.SetActive(false);
    }

    private void OnAttackClicked()
    {
        if (battleManager.activeCharacter != null && battleManager.activeCharacter.isPlayer)
        {
            // Thay đổi: Bây giờ chúng ta sẽ chuyển sang trạng thái chọn mục tiêu
            // BattleManager sẽ xử lý việc hiển thị các lựa chọn mục tiêu
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.targetingState);
        }
    }

    private void OnSkillClicked()
    {
        Debug.Log("Sử dụng Kỹ năng!");
        // Thêm logic cho kỹ năng ở đây
    }

    private void OnParryClicked()
    {
        Debug.Log("Thực hiện Đỡ đòn!");
        // Thêm logic cho đỡ đòn ở đây
    }
}
