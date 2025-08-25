using UnityEngine;

// Trạng thái chờ lượt
public class WaitingState : BaseState
{
    public WaitingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đã bắt đầu chờ lượt.");
    }

    public override void OnUpdate()
    {
        // Loại bỏ logic tự động chuyển trạng thái khỏi đây.
        // WaitingState chỉ đơn giản là chờ đợi.
        // BattleManager sẽ chịu trách nhiệm chuyển trạng thái khi đến lượt của nhân vật.
    }

    public override void OnExit()
    {
        Debug.Log(stateMachine.gameObject.name + " đã kết thúc chờ lượt.");
    }
}
