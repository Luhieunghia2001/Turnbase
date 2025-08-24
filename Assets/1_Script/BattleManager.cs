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

    // --- Các Prefab và vị trí Spawn ---
    public Character playerPrefab;

    // Mảng các vị trí cố định cho kẻ địch, có thể kéo thả từ Inspector
    public Transform[] enemySlots;
    public Character[] enemyPrefabs;

    public Transform playerSpawnPoint;

    // Tham chiếu UI
    public PlayerActionUI playerActionUI;

    void Start()
    {
        // Khởi tạo trận chiến
        SetupBattle();

        // Dùng Coroutine để trì hoãn một chút, đảm bảo tất cả các Start() đã chạy xong
        StartCoroutine(StartTurnCycleAfterDelay());
    }

    IEnumerator StartTurnCycleAfterDelay()
    {
        Debug.Log("Đang đợi một frame để bắt đầu vòng lặp lượt chơi.");
        yield return new WaitForEndOfFrame(); // Đợi đến cuối frame, đảm bảo tất cả các Start() đã hoàn thành
        StartTurnCycle();
        Debug.Log("Vòng lặp lượt chơi đã bắt đầu.");
    }

    void SetupBattle()
    {
        Debug.Log("Đang thiết lập trận chiến.");
        // 1. Spawn nhân vật người chơi
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            Character playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            allCombatants.Add(playerInstance);
            playerInstance.initialPosition = playerSpawnPoint.position;

            // Lấy tham chiếu đến state machine và battle manager
            CharacterStateMachine playerStateMachine = playerInstance.GetComponent<CharacterStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.battleManager = this;
                playerStateMachine.SwitchState(playerStateMachine.waitingState);
            }

            Debug.Log("Người chơi đã được tạo và thêm vào danh sách.");
        }

        // 2. Spawn kẻ địch
        if (enemySlots.Length > 0 && enemyPrefabs.Length > 0)
        {
            for (int i = 0; i < enemySlots.Length && i < enemyPrefabs.Length; i++)
            {
                if (enemySlots[i] != null && enemyPrefabs[i] != null)
                {
                    Character enemyInstance = Instantiate(enemyPrefabs[i], enemySlots[i].position, Quaternion.identity);
                    allCombatants.Add(enemyInstance);
                    enemyInstance.initialPosition = enemySlots[i].position;

                    // Lấy tham chiếu đến state machine và battle manager
                    CharacterStateMachine enemyStateMachine = enemyInstance.GetComponent<CharacterStateMachine>();
                    if (enemyStateMachine != null)
                    {
                        enemyStateMachine.battleManager = this;
                        enemyStateMachine.SwitchState(enemyStateMachine.waitingState);
                    }

                    Debug.Log("Kẻ địch " + enemyInstance.gameObject.name + " đã được tạo.");
                }
                else
                {
                    Debug.LogError("Lỗi: enemySlots[" + i + "] hoặc enemyPrefabs[" + i + "] bị null trong Inspector. Vui lòng kiểm tra lại!");
                }
            }
        }

        // Sắp xếp danh sách
        allCombatants = allCombatants.OrderBy(c => c.transform.position.x).ToList();
        Debug.Log("Danh sách nhân vật đã được sắp xếp. Tổng số nhân vật: " + allCombatants.Count);
    }

    private void StartTurnCycle()
    {
        AdvanceTurn();
    }

    public void AdvanceTurn()
    {
        Debug.Log("Gọi AdvanceTurn().");

        if (activeCharacter != null)
        {
            Debug.Log("Đã có activeCharacter, không chuyển lượt.");
            return;
        }

        foreach (Character combatant in allCombatants)
        {
            // Bổ sung kiểm tra null cho stats để tránh lỗi
            if (combatant.stats == null || !combatant.isAlive) continue;

            // Log trạng thái của từng nhân vật trong vòng lặp
            Debug.Log("Kiểm tra " + combatant.gameObject.name + ". Trạng thái hiện tại: " + combatant.stateMachine.currentState.GetType().Name);

            if (combatant.stateMachine.currentState is WaitingState)
            {
                combatant.stateMachine.SwitchState(combatant.stateMachine.readyState);
                activeCharacter = combatant;
                Debug.Log("Đến lượt của " + activeCharacter.gameObject.name);

                // Hiển thị UI cho người chơi
                if (activeCharacter.isPlayer)
                {
                    playerActionUI.Show();
                }

                return;
            }
        }

        Debug.LogWarning("Không tìm thấy nhân vật nào ở trạng thái WaitingState. activeCharacter vẫn null.");
    }

    public void EndTurn(Character character)
    {
        if (character == activeCharacter)
        {
            activeCharacter = null;
            if (character.stateMachine != null)
            {
                character.stateMachine.SwitchState(character.stateMachine.waitingState);
            }
        }
        AdvanceTurn();
    }

    public void RemoveCombatant(Character character)
    {
        if (allCombatants.Contains(character))
        {
            allCombatants.Remove(character);
        }
    }
}
