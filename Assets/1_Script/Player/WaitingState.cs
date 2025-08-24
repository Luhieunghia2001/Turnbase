// Trạng thái chờ lượt
using UnityEngine;

public class WaitingState : BaseState
{
    private float actionPoints;

    public WaitingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        actionPoints = 0;
        Debug.Log(stateMachine.gameObject.name + " đã bắt đầu chờ lượt.");
    }

    public override void OnUpdate()
    {
        // Tăng điểm hành động dựa trên chỉ số agility
        actionPoints += stateMachine.character.stats.agility * Time.deltaTime;

        if (actionPoints >= 100)
        {
            stateMachine.SwitchState(stateMachine.readyState);
        }
    }
}