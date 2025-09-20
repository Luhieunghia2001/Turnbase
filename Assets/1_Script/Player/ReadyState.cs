using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ReadyState : BaseState
{
    private List<Character> enemies;
    private int currentIndex;

    public ReadyState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log(stateMachine.gameObject.name + " đã sẵn sàng hành động.");


        // Ẩn tất cả các marker trước khi vào trạng thái
        ShowTargetMarker(false);

        stateMachine.character.target = null;

        enemies = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);

        if (enemies.Count > 0)
        {
            currentIndex = 0;
            stateMachine.character.target = enemies[currentIndex];
            // Chỉ hiển thị marker cho mục tiêu mặc định
            if (stateMachine.character.isPlayer)
            {
                if (enemies[currentIndex].targetMarker != null)
                {
                    enemies[currentIndex].targetMarker.SetActive(true);
                }
            }
        }
        else
        {
            stateMachine.character.target = null;
        }
    }

    public override void OnUpdate()
    {
        // Kiểm tra xem nhân vật có phải là người chơi không trước khi xử lý input
        if (stateMachine.character.isPlayer)
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
    }

    private void UpdateTarget(int direction)
    {
        if (enemies.Count > 0)
        {
            // Ẩn marker của mục tiêu hiện tại trước khi chuyển đổi
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(false);
            }

            currentIndex = (currentIndex + direction + enemies.Count) % enemies.Count;
            stateMachine.character.target = enemies[currentIndex];
            Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name + " tại vị trí slot: " + currentIndex);

            // Hiển thị marker cho mục tiêu mới
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(true);
            }
        }
    }

    public override void OnExit()
    {
        // Ẩn tất cả các target marker khi thoát khỏi trạng thái
        ShowTargetMarker(false);
    }

    /// <summary>
    /// Hiển thị hoặc ẩn tất cả các target marker của kẻ địch.
    /// </summary>
    /// <param name="active">Giá trị true để hiển thị, false để ẩn.</param>
    private void ShowTargetMarker(bool active)
    {
        if (enemies != null)
        {
            foreach (Character enemy in enemies)
            {
                if (enemy != null && enemy.targetMarker != null)
                {
                    enemy.targetMarker.SetActive(active);
                }
            }
        }
    }
}
