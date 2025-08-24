using UnityEngine;
using System.Collections; // Cần thiết cho Coroutine

// Lớp này quản lý các trạng thái của nhân vật
public class CharacterStateMachine : MonoBehaviour
{
    // Tham chiếu đến các script khác trên cùng GameObject
    public Character character;

    // Tham chiếu đến BattleManager để truy cập danh sách kẻ địch
    public BattleManager battleManager;

    // Các đối tượng trạng thái
    public BaseState currentState; // Đã đổi từ private thành public
    public WaitingState waitingState;
    public ReadyState readyState;
    public AttackingState attackingState;
    public TakingDamageState takingDamageState;
    public DeadState deadState;
    public ParryingState parryingState;
    public InterruptedState interruptedState;
    public TargetingState targetingState; // Thêm trạng thái chọn mục tiêu

    private void Awake()
    {
        character = GetComponent<Character>();

        // Lấy tham chiếu đến BattleManager trên scene
        battleManager = FindFirstObjectByType<BattleManager>();

        // Khởi tạo tất cả các đối tượng trạng thái
        waitingState = new WaitingState(this);
        readyState = new ReadyState(this);
        attackingState = new AttackingState(this);
        takingDamageState = new TakingDamageState(this);
        deadState = new DeadState(this);
        parryingState = new ParryingState(this);
        interruptedState = new InterruptedState(this);
        targetingState = new TargetingState(this); // Khởi tạo trạng thái mới
    }

    void Start()
    {
        // Chuyển trạng thái ban đầu tại đây để đảm bảo mọi thứ đã sẵn sàng
        SwitchState(waitingState);
    }

    void Update()
    {
        // Gọi hàm OnUpdate của trạng thái hiện tại
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    // Phương thức để chuyển đổi trạng thái
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
