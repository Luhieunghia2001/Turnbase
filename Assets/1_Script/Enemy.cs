using UnityEngine;

// Lớp này đại diện cho một kẻ địch trong trận chiến
// Kế thừa từ Character để tương thích với BattleManager
public class Enemy : Character
{
    // Phương thức này được gọi khi kẻ địch bắt đầu lượt
    public void PerformTurn()
    {
        // Tìm mục tiêu (người chơi)
        Character playerTarget = FindFirstObjectByType<Character>();
        if (playerTarget != null)
        {
            target = playerTarget;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy người chơi để tấn công.");
        }
    }
}
