using UnityEngine;

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
    public GameObject targetMarker; // Thêm trường này để tham chiếu đến GameObject đánh dấu mục tiêu
    
    public Animator animator;


    void Awake()
    {
        // Lấy tham chiếu đến CharacterStateMachine được gắn trên cùng GameObject
        stateMachine = GetComponent<CharacterStateMachine>();
    }
}
