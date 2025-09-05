using UnityEngine;

// Trạng thái chờ lượt
public class WaitingState : BaseState
{    
    public WaitingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log(stateMachine.gameObject.name + " đã bắt đầu chờ lượt.");
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        Debug.Log(stateMachine.gameObject.name + " đã kết thúc chờ lượt.");
    }


}
