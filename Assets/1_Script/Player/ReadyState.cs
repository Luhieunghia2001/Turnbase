using UnityEngine;
public class ReadyState : BaseState
{
    public ReadyState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đã sẵn sàng hành động.");
        // Kích hoạt UI cho người chơi hoặc cho AI hành động (tùy thuộc là Player hay Enemy)
    }

    public override void OnUpdate()
    {
        // Ở đây, chúng ta sẽ chờ input từ người chơi hoặc AI
    }
}