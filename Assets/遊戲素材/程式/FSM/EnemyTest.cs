using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 測試用敵人
/// 站著不動，只接受傷害，有血條 UI
/// </summary>
public class EnemyTest : MonoBehaviour, IDamageable
{
    [Header("血量設定")]
    [SerializeField]
    private float hpMax = 300f;
    private float hp;

    [Header("UI（World Space Canvas 下的元件）")]
    [SerializeField]
    private Image imgHp;            // 血條 Image（Fill Amount）
    [SerializeField]
    private TMP_Text textHp;        // 血量文字

    [Header("血條跟隨攝影機")]
    [SerializeField]
    private Transform hpBarCanvas;  // 血條 Canvas 的 Transform

    private void Start()
    {
        hp = hpMax;
        UpdateHpUI();
    }



    /// <summary>
    /// 實作 IDamageable 介面
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (hp <= 0) return;

        hp -= damage;
        hp = Mathf.Clamp(hp, 0, hpMax);

        // 加這行確認數值
        Debug.Log($"<color=#f66>TakeDamage 呼叫 | hp：{hp} | imgHp 是否為null：{imgHp == null}</color>");

        UpdateHpUI();

        if (hp <= 0)
            Die();
    }

    private void UpdateHpUI()
    {
        Debug.Log($"<color=#ff9>UpdateHpUI | imgHp：{imgHp} | fillAmount：{hp / hpMax}</color>");

        if (imgHp != null)
            imgHp.fillAmount = hp / hpMax;
        if (textHp != null)
            textHp.text = $"{hp} / {hpMax}";
    }

    private void Die()
    {
        Debug.Log($"<color=#f00>{gameObject.name} 死亡</color>");
        // 測試階段直接刪除物件
        // 正式版改為播放死亡動畫
        Destroy(gameObject);
    }
}