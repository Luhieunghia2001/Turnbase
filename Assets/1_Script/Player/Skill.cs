using UnityEngine;

// Thêm dòng này để cho phép tạo ScriptableObject từ Unity Editor
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public int damage;
    public int manaCost;
    public Sprite icon;

    public SkillTargetType targetType;
    public SkillType skillType;
    public ElementType elementType;

}

// Giữ nguyên các enum
public enum SkillTargetType
{
    Self,       // Chỉ bản thân
    Ally,       // Một đồng minh
    Enemy,      // Một kẻ địch
    Enemies,    // Tất cả kẻ địch (Lỗi: 'AllEnemie' -> sửa thành 'Enemies')
    Allies,     // Tất cả đồng minh

}

public enum SkillType
{
    Damage,
    DamageAll,
    Heal,
    Buff,
    Debuff,
    Special
}

public enum ElementType
{
    None,
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}