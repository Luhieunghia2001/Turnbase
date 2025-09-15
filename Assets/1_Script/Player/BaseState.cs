using UnityEngine;

public abstract class BaseState
{
    protected CharacterStateMachine stateMachine;

    public BaseState(CharacterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }

    public virtual void OnUpdate() { }

    public virtual void OnExit() { }
}
