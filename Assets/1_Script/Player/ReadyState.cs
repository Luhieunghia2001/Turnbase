using UnityEngine;
using System.Collections.Generic;

public class ReadyState : BaseState
{
    private List<Character> enemies;
    private int currentIndex;

    public ReadyState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đã sẵn sàng hành động.");

        stateMachine.character.target = null;

        enemies = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);

        if (enemies.Count > 0)
        {
            currentIndex = 0;
            stateMachine.character.target = enemies[currentIndex];
        }
        else
        {
            stateMachine.character.target = null;
        }

        UpdateTargetMarkerDisplay();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateTarget(-1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdateTarget(1);
        }
    }

    private void UpdateTarget(int direction)
    {
        if (enemies.Count > 0)
        {
            currentIndex = (currentIndex + direction + enemies.Count) % enemies.Count;
            stateMachine.character.target = enemies[currentIndex];
            UpdateTargetMarkerDisplay();
            Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name + " tại vị trí slot: " + currentIndex);
        }
    }

    public override void OnExit()
    {
        if (enemies != null)
        {
            foreach (Character enemy in enemies)
            {
                if (enemy != null && enemy.targetMarker != null)
                {
                    enemy.targetMarker.SetActive(false);
                }
            }
        }
    }

    private void UpdateTargetMarkerDisplay()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && enemies[i].targetMarker != null)
            {
                enemies[i].targetMarker.SetActive(i == currentIndex);
            }
        }
    }
}
