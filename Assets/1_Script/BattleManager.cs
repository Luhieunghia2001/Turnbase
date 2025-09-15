using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public List<Character> allCombatants = new List<Character>();
    public Character activeCharacter;

    private bool isProcessingTurn = false;

    [Header("Players")]
    public Character[] playerPrefabs;
    public Transform[] playerSpawnPoints;

    [Header("Enemies")]
    public Transform[] enemySlots;
    public Character[] enemyPrefabs;

    public TurnOrderUI turnOrderUI;

    void Start()
    {
        SetupBattle();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;  // đợi 1 frame
        StartCoroutine(UpdateActionGauge());
    }

    void SetupBattle()
    {
        allCombatants = new List<Character>();

        // Spawn Players
        int playerCount = Mathf.Min(playerPrefabs.Length, playerSpawnPoints.Length);
        for (int i = 0; i < playerCount; i++)
        {
            Character playerInstance = Instantiate(playerPrefabs[i], playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
            playerInstance.transform.SetParent(playerSpawnPoints[i]);

            playerInstance.isPlayer = true;
            allCombatants.Add(playerInstance);
            playerInstance.initialPosition = playerSpawnPoints[i].position;
            playerInstance.battleManager = this;

            CharacterStateMachine playerStateMachine = playerInstance.GetComponent<CharacterStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.battleManager = this;
            }

            // 🔹 Gắn UI riêng cho mỗi player
            PlayerActionUI actionUI = playerInstance.GetComponentInChildren<PlayerActionUI>(true);
            if (actionUI != null)
            {
                // Gán owner để UI biết thuộc về player này
                actionUI.SetOwner(playerInstance);

                // Ẩn UI lúc spawn
                actionUI.Hide();

                // Subscribe parry event
                actionUI.OnParryAttempted += OnParryAttempted;

                // Lưu tham chiếu UI vào player
                playerInstance.ownUI = actionUI;
            }
        }

        // Spawn Enemies
        int enemyCount = Mathf.Min(enemySlots.Length, enemyPrefabs.Length);
        for (int i = 0; i < enemyCount; i++)
        {
            Character enemyInstance = Instantiate(enemyPrefabs[i], enemySlots[i].position, enemySlots[i].rotation);
            enemyInstance.transform.SetParent(enemySlots[i]);

            enemyInstance.isPlayer = false;
            allCombatants.Add(enemyInstance);
            enemyInstance.initialPosition = enemySlots[i].position;
            enemyInstance.battleManager = this;

            CharacterStateMachine enemyStateMachine = enemyInstance.GetComponent<CharacterStateMachine>();
            if (enemyStateMachine != null)
            {
                enemyStateMachine.battleManager = this;
            }
        }

        // Reset state ban đầu
        foreach (Character combatant in allCombatants)
        {
            if (combatant.stateMachine != null)
            {
                combatant.stateMachine.SwitchState(combatant.stateMachine.waitingState);
                combatant.actionGauge = 0;
            }
        }
    }

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
                    var readyCharacters = allCombatants
                        .Where(c => c.actionGauge >= 100 && c.isAlive)
                        .OrderByDescending(c => c.actionGauge)
                        .ToList();

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
        if (activeCharacter != null) return;

        activeCharacter = characterToAct;
        Debug.Log($"Đến lượt: {activeCharacter.gameObject.name}");

        activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.waitingState);

        if (turnOrderUI != null)
        {
            turnOrderUI.HighlightActiveCharacter(activeCharacter);
        }

        if (activeCharacter.isPlayer)
        {
            // Ẩn hết UI của các player khác
            foreach (var player in allCombatants.Where(c => c.isPlayer))
            {
                if (player.ownUI != null) player.ownUI.Hide();
            }

            // Hiện UI đúng cho player đang active
            if (activeCharacter.ownUI != null)
            {
                activeCharacter.ownUI.Show();
                Debug.Log("Trying to show UI: " + activeCharacter.ownUI.playerActionsPanel.activeInHierarchy);
                activeCharacter.ownUI.SetupSkillUI(activeCharacter.skills);
                activeCharacter.ownUI.SetActiveCharacter(activeCharacter);
            }

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

        // Bắt đầu coroutine xử lý parry
        StartCoroutine(EnemyParryWindow(enemy));
    }

    private IEnumerator EnemyParryWindow(Character enemy)
    {
        Character player = allCombatants.FirstOrDefault(c => c.isPlayer && c.isAlive);
        if (player == null) yield break;

        float parryTimer = 0f;
        float attackDuration = 1.5f;

        if (player.ownUI != null)
            player.ownUI.ShowParryUI(true);

        while (parryTimer < attackDuration)
        {
            parryTimer += Time.deltaTime;

            if (player.ownUI != null && player.ownUI.parrySlider != null)
            {
                player.ownUI.parrySlider.value = parryTimer / attackDuration;
            }

            if (player.ownUI != null && player.ownUI.parrySlider.value >= 0.6f && player.ownUI.parrySlider.value <= 0.9f)
            {
                player.isParryable = true;
            }
            else
            {
                player.isParryable = false;
            }

            yield return null;
        }

        if (player.ownUI != null)
            player.ownUI.ShowParryUI(false);
    }

    public void OnParryAttempted()
    {
        Character player = allCombatants.FirstOrDefault(c => c.isPlayer && c.isAlive);
        if (player != null && player.isParryable)
        {
            Debug.Log("Parry thành công!");
            player.isParryable = false;

            if (activeCharacter != null)
            {
                activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.interruptedState);
            }
            player.stateMachine.SwitchState(player.stateMachine.parryingState);
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
                character.actionGauge = 0;
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
