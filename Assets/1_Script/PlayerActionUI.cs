using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerActionUI : MonoBehaviour
{
    public static PlayerActionUI Instance { get; private set; }
    public Character Owner { get; private set; }

    public BattleManager battleManager;

    public Button attackButton;
    public Button skillButton;
    public Button parryButton;
    public Button confirmButton;

    private Character currentCharacter;

    [Header("Skill Buttons")]
    public List<Button> skillButtons;

    public GameObject playerActionsPanel;
    public GameObject PlayerSkill;

    // Thêm tham chiếu đến Slider để quản lý thanh parry
    public Slider parrySlider;

    // Delegate để thông báo cho BattleManager biết khi nào nút Parry được nhấn
    public Action OnParryAttempted;

    private Skill selectedSkillToConfirm;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Add listeners (safe to add once)
        attackButton.onClick.AddListener(OnAttackClicked);
        skillButton.onClick.AddListener(OnSkillClicked);
        parryButton.onClick.AddListener(OnParryClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);

        PlayerSkill.gameObject.SetActive(false);

        // fallback: tìm BattleManager nếu chưa có (SetOwner sẽ gán đúng hơn)
        if (battleManager == null)
        {
            battleManager = FindFirstObjectByType<BattleManager>();
        }

        // ẩn mặc định; BattleManager sẽ show UI khi đến lượt owner
        Hide();
    }

    // Called by BattleManager right after spawn to link UI <-> Character
    public void SetOwner(Character owner)
    {
        Owner = owner;
        currentCharacter = owner;

        // ensure we have battleManager reference
        if (Owner != null)
        {
            battleManager = Owner.battleManager;
        }

        // hide initially
        Hide();
    }

    public void ShowUI()
    {
        StartCoroutine(ShowDelay());
    }

    private IEnumerator ShowDelay()
    {
        yield return new WaitForSeconds(1f);

        playerActionsPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        // ẩn các UI liên quan đến parry (hiện khi cần)
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

    // (Optional) allow BattleManager to set the active character - useful if UI reused
    public void SetActiveCharacter(Character character)
    {
        currentCharacter = character;
    }

    private void OnAttackClicked()
    {
        Debug.Log("OnAttackClicked được gọi.");
        if (currentCharacter != null && currentCharacter.isPlayer)
        {
            currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.readyState);
            confirmButton.gameObject.SetActive(true);
        }

        PlayerSkill.SetActive(false);
    }

    private void OnSkillClicked()
{
    Debug.Log("Sử dụng Kỹ năng!");

        confirmButton.gameObject.SetActive(false);

        if (PlayerSkill.activeSelf == true)
        {
            // Nếu PlayerSkill đang hiển thị (true), thì tắt nó đi
            PlayerSkill.SetActive(false);
        }
        else
        {
            // Nếu PlayerSkill đang tắt (false), thì bật nó lên
            PlayerSkill.SetActive(true);
        }
}

    // IMPORTANT: uses currentCharacter (the owner) instead of battleManager.activeCharacter
    private void OnSkillButtonClicked(Skill selectedSkill)
    {
        if (currentCharacter == null) return;

        Debug.Log($"OnSkillButtonClicked được gọi với kỹ năng: {selectedSkill.skillName} cho {currentCharacter.name}");

        currentCharacter.stateMachine.SwitchState(
            new ReadyStateSkill(currentCharacter.stateMachine, selectedSkill)
        );

        selectedSkillToConfirm = selectedSkill;
        confirmButton.gameObject.SetActive(true);
    }

    public void SetupSkillUI(List<Skill> skills)
    {
        Debug.Log("SetupSkillUI được gọi.");

        // Ẩn tất cả button trước
        foreach (var b in skillButtons) b.gameObject.SetActive(false);

        for (int i = 0; i < skills.Count && i < skillButtons.Count; i++)
        {
            Button currentButton = skillButtons[i];
            currentButton.onClick.RemoveAllListeners();

            Skill skillToUse = skills[i];
            // capture local var for closure
            Skill captured = skillToUse;
            currentButton.onClick.AddListener(() => OnSkillButtonClicked(captured));

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
        // Notify subscribers (BattleManager subscribed at spawn)
        OnParryAttempted?.Invoke();
    }

    private void OnConfirmClicked()
    {
        Debug.Log("OnConfirmClicked được gọi.");

        if (currentCharacter == null) return;

        if (currentCharacter.stateMachine.currentState is ReadyStateSkill currentState)
        {
            Debug.Log("Gọi OnConfirm() của ReadyStateSkill.");
            currentState.OnConfirm();

            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false);
        }
        else if (currentCharacter.stateMachine.currentState is ReadyState)
        {
            Debug.Log("Chuyển từ ReadyState sang AttackingState.");
            currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.attackingState);
            confirmButton.gameObject.SetActive(false);
            PlayerSkill.SetActive(false);
        }
    }
}
