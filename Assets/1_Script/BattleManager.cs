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
            // Thêm log để xác nhận tất cả nhân vật đã được thêm vào và actionGauge đã được đặt lại
            Debug.Log($"Đã thêm {combatant.gameObject.name} vào trận chiến. Action Gauge ban đầu: {combatant.actionGauge}");
        }
    }

    // Coroutine để cập nhật thanh hành động mỗi frame
    private IEnumerator UpdateActionGauge()
    {
        // Thêm độ trễ ban đầu để game khởi tạo đầy đủ
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            // Chỉ chạy khi không có nhân vật nào đang hành động và không có lượt nào đang được xử lý
            if (activeCharacter == null && !isProcessingTurn)
            {
                bool someoneReady = false;
                foreach (var combatant in allCombatants)
                {
                    if (combatant.isAlive)
                    {
                        combatant.actionGauge += combatant.stats.agility * Time.deltaTime;

                        // Log giá trị actionGauge của mỗi nhân vật

                        // Kiểm tra nếu có ai đó đã sẵn sàng để hành động
                        if (combatant.actionGauge >= 100 && combatant.stateMachine.currentState is WaitingState)
                        {
                            someoneReady = true;
                        }
                    }
                }

                // Cập nhật giao diện UI của TurnOrder
                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                if (someoneReady)
                {
                    // Đặt cờ để ngăn các lượt khác chạy song song
                    isProcessingTurn = true;
                    // Lấy danh sách các nhân vật đã sẵn sàng và sắp xếp theo thanh hành động giảm dần
                    var readyCharacters = allCombatants.Where(c => c.actionGauge >= 100 && c.isAlive).OrderByDescending(c => c.actionGauge).ToList();
                    if (readyCharacters.Any())
                    {
                        // Log nhân vật chuẩn bị hành động và actionGauge của họ
                        Debug.Log($"Nhân vật sắp hành động: {readyCharacters.First().gameObject.name} với actionGauge: {readyCharacters.First().actionGauge}");
                        // Chuyển lượt cho nhân vật có thanh hành động cao nhất
                        AdvanceTurn(readyCharacters.First());
                    }
                }
            }
            yield return null;
        }
    }

    public void AdvanceTurn(Character characterToAct)
    {
        // activeCharacter được đặt trong phương thức này, nên chỉ cần kiểm tra một lần duy nhất
        if (activeCharacter != null)
        {
            return;
        }

        activeCharacter = characterToAct;
        Debug.Log($"Kiểm tra nhân vật: {activeCharacter.gameObject.name}");

        // Chuyển sang trạng thái sẵn sàng
        activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.readyState);

        // Cập nhật giao diện người dùng
        if (turnOrderUI != null)
        {
            turnOrderUI.HighlightActiveCharacter(activeCharacter);
        }

        if (activeCharacter.isPlayer)
        {
            playerActionUI.Show();
        }
        else // Đây là lượt của kẻ địch
        {
            StartCoroutine(EnemyTurn(activeCharacter)); // Truyền activeCharacter vào coroutine
        }
    }

    // Coroutine mới để xử lý lượt của kẻ địch
    private IEnumerator EnemyTurn(Character enemy)
    {
        Debug.Log("Đến lượt của kẻ địch: " + enemy.gameObject.name);

        // Chờ một chút để người chơi thấy lượt của kẻ địch
        yield return new WaitForSeconds(1f);

        // Giả sử kẻ địch có một phương thức để thực hiện hành động
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.PerformTurn();
        }

        // Chờ một chút trước khi kết thúc lượt
        yield return new WaitForSeconds(2f);

        EndTurn(enemy);
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
            // Đặt lại cờ để cho phép lượt tiếp theo bắt đầu
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
