using UnityEngine;

/// <summary>
/// 玩家特殊招式狀態
/// 規則：
///   - 單鍵觸發（O 鍵 / 滑鼠中鍵）
///   - 四種武器共用同一個 CD
///   - 可以打斷普通攻擊與連段
///   - 特殊招式本身不可被普通攻擊打斷
///   - 有無敵幀（大部分，動畫最後幾幀沒有）
/// </summary>
public class PlayerSpecialAttack : PlayerState
{
    // ────────────────────────────────────────
    // CD 設定（四種武器共用）
    // ────────────────────────────────────────

    [UnityEngine.Tooltip("特殊招式冷卻時間（秒），四種武器共用")]
    private float cooldown = 5.0f;          // 共用 CD 時間
    private float lastUseTime = -999f;      // 上次使用時間，初始為負數確保一開始可以使用

    // ────────────────────────────────────────
    // 狀態旗標
    // ────────────────────────────────────────

    public bool isSpecialAttacking { get; private set; }  // 是否在特殊招式中

    // 模擬特殊招式動畫持續時間（有動畫後改為讀取動畫長度）
    private float specialDuration = 1.5f;

    // ────────────────────────────────────────
    // 建構函式
    // ────────────────────────────────────────

    public PlayerSpecialAttack(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    // ────────────────────────────────────────
    // CD 查詢（由 Player.InputSpecialAttack() 使用）
    // ────────────────────────────────────────

    /// <summary>
    /// 是否可以使用特殊招式（CD 是否結束）
    /// </summary>
    public bool CanUse()
    {
        return Time.time - lastUseTime >= cooldown;
    }

    /// <summary>
    /// 目前 CD 剩餘時間
    /// </summary>
    public float CooldownRemaining()
    {
        float remaining = cooldown - (Time.time - lastUseTime);
        return Mathf.Max(0f, remaining);
    }

    // ────────────────────────────────────────
    // 狀態機標準方法
    // ────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();
        isSpecialAttacking = true;
        lastUseTime = Time.time;    // 進入狀態時開始計算 CD

        // player.ani.SetTrigger(...);  // 有動畫後取消註解

        Debug.Log($"<color=#f0f> 觸發特殊招式 | 武器：{player.currentWeapon}" +
                  $" | CD：{cooldown} 秒</color>");
    }

    public override void Exit()
    {
        base.Exit();
        isSpecialAttacking = false;
        Debug.Log("<color=#f0f>特殊招式結束</color>");
    }

    public override void Update()
    {
        base.Update();

        // 有動畫後改為：timer >= player.ani.GetCurrentAnimatorStateInfo(0).length
        if (timer >= specialDuration)
            stateMachine.SwitchState(player.idle);
    }
}