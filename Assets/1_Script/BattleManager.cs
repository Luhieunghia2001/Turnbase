using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Danh sách tất cả các nhân vật tham gia trận chiến (bao gồm cả người chơi và kẻ địch)
    public List<Character> allCombatants;

    // Biến để theo dõi lượt của ai
    public Character activeCharacter;

    // --- Các Prefab và vị trí Spawn ---
    public Character playerPrefab;
    public List<Character> enemyPrefabs;

    public Transform playerSpawnPoint;
    public List<Transform> enemySpawnPoints;

    // Tham chiếu UI
    public PlayerActionUI playerActionUI;

    void Start()
    {
        // Gọi phương thức để thiết lập trận chiến
        SetupBattle();
    }

    void SetupBattle()
    {
        // 1. Spawn nhân vật người chơi
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            Character playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            allCombatants.Add(playerInstance);
        }

        // 2. Spawn danh sách quái vật ngẫu nhiên (hoặc cố định)
        for (int i = 0; i < enemySpawnPoints.Count && i < enemyPrefabs.Count; i++)
        {
            Character enemyInstance = Instantiate(enemyPrefabs[i], enemySpawnPoints[i].position, Quaternion.identity);
            allCombatants.Add(enemyInstance);
        }
    }

    // Phương thức để tìm nhân vật sẵn sàng hành động
    void Update()
    {
        // Nếu không có nhân vật nào đang hành động
        if (activeCharacter == null)
        {
            // Tìm nhân vật đầu tiên đã Ready và kích hoạt họ
            foreach (Character combatant in allCombatants)
            {
                if (combatant.stateMachine != null && combatant.stateMachine.currentState is ReadyState)
                {
                    activeCharacter = combatant;
                    Debug.Log("Đến lượt của " + activeCharacter.gameObject.name);

                    // Hiển thị UI cho người chơi
                    if (activeCharacter.isPlayer)
                    {
                        // Chuyển sang trạng thái chọn mục tiêu ngay lập tức
                        activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.targetingState);
                    }

                    break;
                }
            }
        }
    }

    // Phương thức để kết thúc lượt của nhân vật hiện tại
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
    }

    // Phương thức để loại bỏ nhân vật khi họ bị hạ gục
    public void RemoveCombatant(Character character)
    {
        if (allCombatants.Contains(character))
        {
            allCombatants.Remove(character);
        }
    }
}
