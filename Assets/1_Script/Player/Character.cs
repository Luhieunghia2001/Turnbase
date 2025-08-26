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
    public GameObject targetMarker;

    public Animator animator;

    public float actionGauge;


    public Vector3 initialPosition;

    public BattleManager battleManager;

    public bool isAlive
    {
        get { return stats.currentHP > 0; }
    }


    void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        animator = GetComponent<Animator>();
    }
}
