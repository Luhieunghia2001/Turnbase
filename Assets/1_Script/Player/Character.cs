using UnityEngine;
using System.Collections.Generic;

public enum BattleState
{
    Waiting,
    Ready,
    Attacking,
    TakingDamage,
    Dead,
    Parrying,
    Interrupted
}

[System.Serializable]
public class CharacterStats
{
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int attack;
    public int defense;
    public int magicAttack;
    public int magicDefense;
    public int agility;
}

public class Character : MonoBehaviour
{
    public CharacterStateMachine stateMachine;

    public CharacterStats stats;
    public bool isPlayer;
    public Character target;
    public GameObject targetMarker;

    public Animator animator;

    public float actionGauge;

    public List<Skill> skills;

    public Vector3 initialPosition;

    public BattleManager battleManager;

    public bool isParryable;

    public bool isAlive
    {
        get { return stats.currentHP > 0; }
    }


    void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damageAmount)
    {
        // Trừ máu
        stats.currentHP -= damageAmount;
        Debug.Log(gameObject.name + " đã nhận " + damageAmount + " sát thương. Máu còn lại: " + stats.currentHP);

        // Kiểm tra xem nhân vật có chết không
        if (stats.currentHP <= 0)
        {
            stats.currentHP = 0;
            // Nếu chết, chuyển sang trạng thái Dead
            stateMachine.SwitchState(stateMachine.deadState);

            // QUAN TRỌNG: Thông báo cho BattleManager để xóa nhân vật này khỏi danh sách.
            // Điều này ngăn ngừa các lỗi NullReferenceException sau này.
            if (battleManager != null)
            {
                battleManager.RemoveCombatant(this);
            }
        }
        else
        {
            // Nếu không chết, chuyển sang trạng thái TakingDamage
            stateMachine.SwitchState(stateMachine.takingDamageState);
        }
    }
}
