using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public List<Character> allCombatants = new List<Character>();

    public Character activeCharacter;

    public Character playerPrefab;

    public Transform[] enemySlots;
    public Character[] enemyPrefabs;

    public Transform playerSpawnPoint;

    public PlayerActionUI playerActionUI;

    void Start()
    {
        SetupBattle();

        StartCoroutine(StartTurnCycleAfterDelay());
    }

    IEnumerator StartTurnCycleAfterDelay()
    {
        Debug.Log("Đang đợi một frame để bắt đầu vòng lặp lượt chơi.");
        yield return new WaitForEndOfFrame(); 
        StartTurnCycle();
        Debug.Log("Vòng lặp lượt chơi đã bắt đầu.");
    }

    void SetupBattle()
    {
        Debug.Log("Đang thiết lập trận chiến.");
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            Character playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            allCombatants.Add(playerInstance);
            playerInstance.initialPosition = playerSpawnPoint.position;

            CharacterStateMachine playerStateMachine = playerInstance.GetComponent<CharacterStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.battleManager = this;
                playerStateMachine.SwitchState(playerStateMachine.waitingState);
            }

            Debug.Log("Người chơi đã được tạo và thêm vào danh sách.");
        }

        if (enemySlots.Length > 0 && enemyPrefabs.Length > 0)
        {
            for (int i = 0; i < enemySlots.Length && i < enemyPrefabs.Length; i++)
            {
                if (enemySlots[i] != null && enemyPrefabs[i] != null)
                {
                    Character enemyInstance = Instantiate(enemyPrefabs[i], enemySlots[i].position, Quaternion.identity);
                    allCombatants.Add(enemyInstance);
                    enemyInstance.initialPosition = enemySlots[i].position;

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
            if (combatant.stats == null || !combatant.isAlive) continue;

            Debug.Log("Kiểm tra " + combatant.gameObject.name + ". Trạng thái hiện tại: " + combatant.stateMachine.currentState.GetType().Name);

            if (combatant.stateMachine.currentState is WaitingState)
            {
                combatant.stateMachine.SwitchState(combatant.stateMachine.readyState);
                activeCharacter = combatant;
                Debug.Log("Đến lượt của " + activeCharacter.gameObject.name);

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
