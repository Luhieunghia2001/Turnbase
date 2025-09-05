using UnityEngine;

// Thêm dòng này để cho phép tạo ScriptableObject từ Unity Editor
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public int damage;
    public int manaCost;
    public SkillTargetType targetType;
    public Sprite icon;
    public SkillType skillType;

}

// Giữ nguyên các enum
public enum SkillTargetType
{
    Self,       // Chỉ bản thân
    Ally,       // Một đồng minh
    Enemy,      // Một kẻ địch
    Enemies,    // Tất cả kẻ địch (Lỗi: 'AllEnemie' -> sửa thành 'Enemies')
    Allies,     // Tất cả đồng minh
    AllEnemie,  // Tất cả kẻ địch
}

public enum SkillType
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Special
}