using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Trạng thái sẵn sàng hành động, giờ có cả chức năng chọn mục tiêu
public class ReadyState : BaseState
{
    private List<Character> enemies;
    private int currentIndex;

    public ReadyState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đã sẵn sàng hành động.");

        // Lấy danh sách kẻ địch
        enemies = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);

        // Đặt mục tiêu mặc định là kẻ địch đầu tiên
        if (enemies.Count > 0)
        {
            currentIndex = 0;
            stateMachine.character.target = enemies[currentIndex];
        }
        else
        {
            stateMachine.character.target = null;
        }

        // Hiển thị dấu hiệu mục tiêu
        UpdateTargetMarkerDisplay();
    }

    public override void OnUpdate()
    {
        // Xử lý phím 'A' để chuyển đến mục tiêu trước đó
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (enemies.Count > 0)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = enemies.Count - 1;
                }
                stateMachine.character.target = enemies[currentIndex];
                UpdateTargetMarkerDisplay();
                Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name + " tại vị trí slot: " + currentIndex);
            }
        }

        // Xử lý phím 'D' để chuyển đến mục tiêu kế tiếp
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (enemies.Count > 0)
            {
                currentIndex++;
                if (currentIndex >= enemies.Count)
                {
                    currentIndex = 0;
                }
                stateMachine.character.target = enemies[currentIndex];
                UpdateTargetMarkerDisplay();
                Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name + " tại vị trí slot: " + currentIndex);
            }
        }

        // Logic tấn công bằng phím Enter và chuột đã được loại bỏ.
    }

    public override void OnExit()
    {
        // Ẩn tất cả các dấu hiệu mục tiêu khi thoát trạng thái
        if (enemies != null)
        {
            foreach (Character enemy in enemies)
            {
                if (enemy.targetMarker != null)
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
            if (enemies[i].targetMarker != null)
            {
                enemies[i].targetMarker.SetActive(i == currentIndex);
            }
        }
    }
}
