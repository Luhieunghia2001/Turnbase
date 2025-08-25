using UnityEngine;

public class Enemy : Character
{
    public void PerformTurn()
    {
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
