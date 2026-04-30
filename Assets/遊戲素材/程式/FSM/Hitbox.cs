using UnityEngine;

/// <summary>
/// 武器傷害碰撞盒
/// 攻擊狀態進入時啟用，結束時關閉
/// 碰到有 IDamageable 介面的物件就造成傷害
/// </summary>
public class Hitbox : MonoBehaviour
{
    [Header("傷害設定")]
    [SerializeField]
    private float damage = 50f;             // 此 Hitbox 的傷害值

    private Collider hitCollider;           // 此物件的碰撞盒
    private bool hasHit = false;            // 同一次攻擊是否已造成傷害（避免重複傷害）

    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
        // 確認 Collider 是 Trigger 模式
        hitCollider.isTrigger = true;
        // 預設關閉
        SetActive(false);
    }

    /// <summary>
    /// 啟用或關閉 Hitbox
    /// </summary>
    public void SetActive(bool active)
    {
        hitCollider.enabled = active;
        // 每次啟用時重置已擊中旗標
        if (active) hasHit = false;
        Debug.Log($"<color=#ff9>Hitbox {(active ? "啟用" : "關閉")}</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=#9cf>Hitbox 碰撞：{other.name} | hasHit:{hasHit}</color>");
        // 已經打到過就跳出（同一次攻擊只造成一次傷害）
        if (hasHit) return;

        // 嘗試取得 IDamageable 介面
        if (other.TryGetComponent(out IDamageable damageable))
        {
            hasHit = true;
            damageable.TakeDamage(damage);
            Debug.Log($"<color=#f90>Hitbox 命中：{other.name} | 傷害：{damage}</color>");
        }
    }

    /// <summary>
    /// 重置命中旗標（連段下一段開始時呼叫）
    /// </summary>
    public void ResetHit()
    {
        hasHit = false;
        Debug.Log("<color=#ff9>Hitbox 重置命中旗標</color>");
    }
}
