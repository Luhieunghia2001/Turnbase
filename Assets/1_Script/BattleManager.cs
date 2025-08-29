using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    // Danh sách tất cả các nhân vật tham gia trận chiến (bao gồm cả người chơi và kẻ địch)
    public List<Character> allCombatants = new List<Character>();

    // Biến để theo dõi lượt của ai
    public Character activeCharacter;

    // Thêm biến cờ để tránh xử lý nhiều lượt cùng lúc
    private bool isProcessingTurn = false;

    // --- Các Prefab và vị trí Spawn ---
    public Character playerPrefab;

    // Mảng các vị trí cố định cho kẻ địch, có thể kéo thả từ Inspector
    public Transform[] enemySlots;
    public Character[] enemyPrefabs;

    public Transform playerSpawnPoint;

    // Tham chiếu UI
    public PlayerActionUI playerActionUI;
    public TurnOrderUI turnOrderUI; // Thêm tham chiếu đến script UI mới

    void Start()
    {
        // Khởi tạo trận chiến
        SetupBattle();
        // Bắt đầu Coroutine sau khi tất cả các nhân vật đã được khởi tạo
        StartCoroutine(UpdateActionGauge());
    }

    void SetupBattle()
    {
        // Khởi tạo danh sách nhân vật
        allCombatants = new List<Character>();

        // 1. Spawn nhân vật người chơi
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            Character playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            playerInstance.transform.SetParent(playerSpawnPoint);
            if (playerInstance != null)
            {
                playerInstance.isPlayer = true; // Thêm dòng này để đánh dấu là người chơi
                allCombatants.Add(playerInstance);
                playerInstance.initialPosition = playerSpawnPoint.position;
                playerInstance.battleManager = this; // Gán tham chiếu BattleManager

                // Gán tham chiếu cho state machine
                CharacterStateMachine playerStateMachine = playerInstance.GetComponent<CharacterStateMachine>();
                if (playerStateMachine != null)
                {
                    playerStateMachine.battleManager = this;
                }
            }
        }

        // 2. Spawn kẻ địch
        if (enemySlots.Length > 0 && enemyPrefabs.Length > 0)
        {
            for (int i = 0; i < enemySlots.Length && i < enemyPrefabs.Length; i++)
            {
                if (enemySlots[i] != null && enemyPrefabs[i] != null)
                {
                    Character enemyInstance = Instantiate(enemyPrefabs[i], enemySlots[i].position, enemySlots[i].rotation);
                    enemyInstance.transform.SetParent(enemySlots[i]);
                    if (enemyInstance != null)
                    {
                        enemyInstance.isPlayer = false; // Thêm dòng này để đánh dấu là kẻ địch
                        allCombatants.Add(enemyInstance);
                        enemyInstance.initialPosition = enemySlots[i].position;
                        enemyInstance.battleManager = this; // Gán tham chiếu BattleManager

                        // Gán tham chiếu cho state machine
                        CharacterStateMachine enemyStateMachine = enemyInstance.GetComponent<CharacterStateMachine>();
                        if (enemyStateMachine != null)
                        {
                            enemyStateMachine.battleManager = this;
                        }
                    }
                }
            }
        }

        // Khởi tạo trạng thái ban đầu cho tất cả nhân vật
        foreach (Character combatant in allCombatants)
        {
            if (combatant.stateMachine != null)
            {
                combatant.stateMachine.SwitchState(combatant.stateMachine.waitingState);
                combatant.actionGauge = 0; // Đặt lại thanh hành động
            }
        }

        // Lắng nghe sự kiện từ PlayerActionUI
        playerActionUI.OnParryAttempted += OnParryAttempted;
    }

    // Coroutine để cập nhật thanh hành động mỗi frame
    private IEnumerator UpdateActionGauge()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (activeCharacter == null && !isProcessingTurn)
            {
                bool someoneReady = false;
                foreach (var combatant in allCombatants)
                {
                    if (combatant.isAlive)
                    {
                        combatant.actionGauge += combatant.stats.agility * Time.deltaTime;

                        if (combatant.actionGauge >= 100 && combatant.stateMachine.currentState is WaitingState)
                        {
                            someoneReady = true;
                        }
                    }
                }

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                if (someoneReady)
                {
                    isProcessingTurn = true;
                    var readyCharacters = allCombatants.Where(c => c.actionGauge >= 100 && c.isAlive).OrderByDescending(c => c.actionGauge).ToList();
                    if (readyCharacters.Any())
                    {
                        AdvanceTurn(readyCharacters.First());
                    }
                }
            }
            yield return null;
        }
    }

    public void AdvanceTurn(Character characterToAct)
    {
        if (activeCharacter != null)
        {
            return;
        }

        activeCharacter = characterToAct;
        Debug.Log($"Kiểm tra nhân vật: {activeCharacter.gameObject.name}");

        activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.readyState);

        if (turnOrderUI != null)
        {
            turnOrderUI.HighlightActiveCharacter(activeCharacter);
        }

        if (activeCharacter.isPlayer)
        {
            playerActionUI.Show();
        }
        else
        {
            StartCoroutine(EnemyTurn(activeCharacter));
        }
    }

    private IEnumerator EnemyTurn(Character enemy)
    {
        Debug.Log("Đến lượt của kẻ địch: " + enemy.gameObject.name);

        yield return new WaitForSeconds(1f);

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.PerformTurn();
        }

        // Bắt đầu coroutine để xử lý thanh parry
        StartCoroutine(EnemyParryWindow(enemy));

        // Wait for the enemy's turn to finish.
        // The EndTurn call will be handled by the AttackingState,
        // or by the Parry logic if successful.
    }

    private IEnumerator EnemyParryWindow(Character enemy)
    {
        // Find player to parry
        Character player = allCombatants.FirstOrDefault(c => c.isPlayer && c.isAlive);
        if (player == null)
        {
            // No player to parry, end coroutine
            yield break;
        }

        float parryTimer = 0f;
        float attackDuration = 1.5f;

        playerActionUI.ShowParryUI(true);

        while (parryTimer < attackDuration)
        {
            parryTimer += Time.deltaTime;
            playerActionUI.parrySlider.value = parryTimer / attackDuration;

            if (playerActionUI.parrySlider.value >= 0.6f && playerActionUI.parrySlider.value <= 0.9f)
            {
                player.isParryable = true;
            }
            else
            {
                player.isParryable = false;
            }

            yield return null;
        }

        playerActionUI.ShowParryUI(false);
    }

    // Phương thức xử lý khi người chơi cố gắng parry
    public void OnParryAttempted()
    {
        Character player = allCombatants.FirstOrDefault(c => c.isPlayer && c.isAlive);
        if (player != null && player.isParryable)
        {
            // Parry thành công!
            Debug.Log("Parry thành công!");
            // Đặt lại cờ
            player.isParryable = false;
            // Chuyển trạng thái của kẻ tấn công sang Interrupted
            if (activeCharacter != null)
            {
                activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.interruptedState);
            }
            // Chuyển trạng thái của người chơi sang Parrying
            player.stateMachine.SwitchState(player.stateMachine.parryingState);
            // Có thể thêm hiệu ứng ở đây
        }
        else
        {
            Debug.Log("Parry không thành công!");
        }
    }

    public void EndTurn(Character character)
    {
        if (character == activeCharacter)
        {
            activeCharacter = null;
            if (character.stateMachine != null)
            {
                character.stateMachine.SwitchState(character.stateMachine.waitingState);
                character.actionGauge = 0; // Đặt lại thanh hành động
            }
            isProcessingTurn = false;
        }
    }

    public void RemoveCombatant(Character character)
    {
        if (allCombatants.Contains(character))
        {
            allCombatants.Remove(character);
        }
    }
}
