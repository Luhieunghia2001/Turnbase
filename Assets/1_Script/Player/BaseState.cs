using UnityEngine;

// Lớp trừu tượng này làm khuôn mẫu cho tất cả các trạng thái
public abstract class BaseState
{
    // Tham chiếu đến State Machine để các trạng thái có thể thay đổi trạng thái khác
    protected CharacterStateMachine stateMachine;

    // Constructor để gán tham chiếu
    public BaseState(CharacterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // Phương thức sẽ được gọi khi vào trạng thái
    public virtual void OnEnter() { }

    // Phương thức sẽ được gọi mỗi frame khi ở trong trạng thái
    public virtual void OnUpdate() { }

    // Phương thức sẽ được gọi khi rời khỏi trạng thái
    public virtual void OnExit() { }
}
