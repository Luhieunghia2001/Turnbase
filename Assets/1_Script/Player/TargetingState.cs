using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Trạng thái chọn mục tiêu
public class TargetingState : BaseState
{
    private List<Character> enemies;
    private int currentIndex;

    public TargetingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log("Chọn mục tiêu tấn công...");
        // Lấy danh sách kẻ địch
        enemies = stateMachine.battleManager.allCombatants.FindAll(c => !c.isPlayer);

        // --- CẬP NHẬT LOGIC KHỞI TẠO ---
        // Đặt mục tiêu mặc định là kẻ địch ở slot 1 (index 0)
        // Đây là cách đơn giản và an toàn nhất để đảm bảo luôn có mục tiêu mặc định
        if (enemies.Count > 0)
        {
            currentIndex = 0;
            // Gán mục tiêu cho nhân vật
            stateMachine.character.target = enemies[currentIndex];
        }
        else
        {
            // Nếu không có kẻ địch nào, thoát trạng thái
            stateMachine.character.target = null;
            stateMachine.SwitchState(stateMachine.readyState);
            return;
        }

        // --- LOG ĐỂ KIỂM TRA ---
        Debug.Log("Tìm thấy " + enemies.Count + " kẻ địch.");
        Debug.Log("Mục tiêu mặc định được chọn là: " + stateMachine.character.target.gameObject.name);

        // Hiển thị dấu hiệu mục tiêu
        UpdateTargetMarkerDisplay();
    }

    public override void OnUpdate()
    {
        // Xử lý phím 'A' để chuyển đến mục tiêu trước đó
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = enemies.Count - 1;
            }
            UpdateTargetMarkerDisplay();
            stateMachine.character.target = enemies[currentIndex];
            // --- LOG ĐỂ KIỂM TRA ---
            Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name);
        }

        // Xử lý phím 'D' để chuyển đến mục tiêu kế tiếp
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex++;
            if (currentIndex >= enemies.Count)
            {
                currentIndex = 0;
            }
            UpdateTargetMarkerDisplay();
            stateMachine.character.target = enemies[currentIndex];
            // --- LOG ĐỂ KIỂM TRA ---
            Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name);
        }

        // Lựa chọn mục tiêu bằng chuột trái (vẫn giữ để linh hoạt)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Character target = hit.collider.GetComponent<Character>();
                if (target != null && !target.isPlayer)
                {
                    stateMachine.character.target = target;
                    stateMachine.SwitchState(stateMachine.readyState);
                }
            }
        }
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
