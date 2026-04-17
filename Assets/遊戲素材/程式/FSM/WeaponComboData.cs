using System.Collections.Generic;

/// <summary>
/// 單一連段的資料定義
/// </summary>
public class ComboData
{
    public string comboName;                    // 連段名稱，Debug 用
    public List<AttackInputType> sequence;      // 連段的按鍵序列
    public WeaponType weapon;                   // 屬於哪把武器

    public ComboData(WeaponType weapon, string name, List<AttackInputType> seq)
    {
        this.weapon = weapon;
        comboName = name;
        sequence = seq;
    }
}