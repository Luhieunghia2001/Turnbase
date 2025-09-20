using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Enemy : Character
{
    public GameObject CanvasUIE;


    public void PerformTurn()
    {
        // 1. Tìm mục tiêu (chọn một người chơi ngẫu nhiên)
        Character playerTarget = FindPlayerTarget();

        if (playerTarget != null)
        {
            // 2. Gán mục tiêu đã tìm thấy cho nhân vật
            // Bây giờ, AttackingState sẽ truy cập mục tiêu này thông qua character.target
            this.target = playerTarget;

            Debug.Log(gameObject.name + " đã tìm thấy mục tiêu: " + playerTarget.gameObject.name);

            // 3. Chuyển sang trạng thái tấn công
            // Mọi logic tấn công và kết thúc lượt sẽ được xử lý trong AttackingState
            stateMachine.SwitchState(stateMachine.attackingState);

        }
        else
        {
            // Nếu không tìm thấy mục tiêu (người chơi đã chết hết), kết thúc trận chiến
            Debug.Log("Không tìm thấy người chơi để tấn công. Trận chiến kết thúc!");
            // TODO: Thêm logic kết thúc trận chiến ở đây
            stateMachine.battleManager.EndTurn(this);
        }
    }

    public void ShowUI()
    {
        CanvasUIE.SetActive(true);
    }

    public void HideUI()
    {
        CanvasUIE.SetActive(false);
    }

    // Phương thức tìm kiếm mục tiêu người chơi
    private Character FindPlayerTarget()
    {
        // Lấy danh sách tất cả các nhân vật đang sống
        var aliveCombatants = battleManager.allCombatants.Where(c => c.isAlive).ToList();

        // Tìm tất cả người chơi trong danh sách đó
        List<Character> players = aliveCombatants.Where(c => c.isPlayer).ToList();

        if (players.Count > 0)
        {
            // Chọn một người chơi ngẫu nhiên từ danh sách
            int randomIndex = Random.Range(0, players.Count);
            return players[randomIndex];
        }
        return null;
    }
}
