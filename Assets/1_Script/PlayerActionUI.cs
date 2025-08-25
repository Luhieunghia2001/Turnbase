using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    public BattleManager battleManager;

    public Button attackButton;
    public Button skillButton;
    public Button parryButton;

    public GameObject playerActionsPanel;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackClicked);
        skillButton.onClick.AddListener(OnSkillClicked);
        parryButton.onClick.AddListener(OnParryClicked);
    }

    public void Show()
    {
        playerActionsPanel.SetActive(true);
    }

    public void Hide()
    {
        playerActionsPanel.SetActive(false);
    }

    private void OnAttackClicked()
    {
        if (battleManager.activeCharacter != null && battleManager.activeCharacter.isPlayer)
        {
            battleManager.activeCharacter.stateMachine.SwitchState(battleManager.activeCharacter.stateMachine.attackingState);
        }
    }

    private void OnSkillClicked()
    {
        Debug.Log("Sử dụng Kỹ năng!");
    }

    private void OnParryClicked()
    {
        Debug.Log("Thực hiện Đỡ đòn!");
    }
}
