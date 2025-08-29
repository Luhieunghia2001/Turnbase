using System.Collections; 
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    public Character character;

    public BattleManager battleManager;

    public BaseState currentState; 
    public WaitingState waitingState;
    public ReadyState readyState;
    public AttackingState attackingState;
    public TakingDamageState takingDamageState;
    public DeadState deadState;
    public ParryingState parryingState;
    public InterruptedState interruptedState;


    private void Awake()
    {
        character = GetComponent<Character>();

        battleManager = FindFirstObjectByType<BattleManager>();
        waitingState = new WaitingState(this);
        readyState = new ReadyState(this);
        attackingState = new AttackingState(this);
        takingDamageState = new TakingDamageState(this);
        deadState = new DeadState(this);
        parryingState = new ParryingState(this);
        interruptedState = new InterruptedState(this);
    }

    void Start()
    {
        SwitchState(waitingState);

    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void SwitchState(BaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }


}
