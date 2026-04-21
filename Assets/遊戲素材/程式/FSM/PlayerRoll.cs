using UnityEngine;

/// <summary>
/// 玩家翻滾
/// 規則：
///   - 向角色當下面對的方向翻滾
///   - 只能在地面使用
///   - 可連續施放（翻滾結束前再按可以立刻再滾）
///   - 僅可被特殊招式打斷，不可被普通攻擊打斷
///   - 大部分期間有無敵幀（測試階段用 Debug.Log 標示）
/// </summary>
public class PlayerRoll : PlayerState
{
    // ────────────────────────────────────────
    // 翻滾參數
    // ────────────────────────────────────────

    // 翻滾持續時間（有動畫後改為讀取動畫長度）
    private float rollDuration = 0.6f;

    // 翻滾速度（比跑步快）
    private float rollSpeed = 8f;

    // 無敵幀佔整體翻滾時間的比例
    // 文件：大部分有無敵幀，最後幾幀沒有
    // 例：0.8 = 前 80% 有無敵幀，後 20% 沒有
    private float invincibleRatio = 0.8f;

    // 起滾時鎖定的方向
    private Vector3 lockedRollDir;

    // 是否正在翻滾中（外部用來擋攻擊輸入）
    public bool isRolling { get; private set; }

    // 目前是否在無敵幀中（之後接傷害系統時使用）
    public bool isInvincible { get; private set; }

    // 是否有待處理的連續翻滾請求
    private bool nextRollQueued;

    // ────────────────────────────────────────
    // 建構函式
    // ────────────────────────────────────────

    public PlayerRoll(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    // ────────────────────────────────────────
    // 接收連續翻滾請求（由 Player.InputRoll() 呼叫）
    // ────────────────────────────────────────

    /// <summary>
    /// 翻滾中再次按下 Shift，排隊下一次翻滾
    /// </summary>
    public void QueueNextRoll()
    {
        nextRollQueued = true;
        Debug.Log("<color=#9f9>翻滾：下一次翻滾已排隊</color>");
    }

    // ────────────────────────────────────────
    // 狀態機標準方法
    // ────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();

        isRolling = true;
        isInvincible = true;
        nextRollQueued = false;

        // 鎖定起滾時角色面對的方向
        lockedRollDir = player.transform.forward;

        // 施加翻滾速度
        player.SetVelocity(
            lockedRollDir * rollSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        // player.ani.SetTrigger("翻滾");  // 有動畫後取消註解

        Debug.Log($"<color=#9f9>開始翻滾 | 方向：{lockedRollDir} | 無敵幀：開啟</color>");
    }

    public override void Exit()
    {
        base.Exit();

        isRolling = false;
        isInvincible = false;
        nextRollQueued = false;

        // 離開時清除水平速度，避免殘留
        player.SetVelocity(new Vector3(
            0,
            player.rig.linearVelocity.y,
            0));

        Debug.Log("<color=#9f9>翻滾結束</color>");
    }

    public override void Update()
    {
        base.Update();

        // 持續施加翻滾速度（方向鎖定不變）
        player.SetVelocity(
            lockedRollDir * rollSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        // ── 無敵幀管理 ──────────────────────────────
        // 前 invincibleRatio 比例的時間有無敵幀
        // 後段沒有無敵幀（讓玩家需要考慮翻滾時機）
        float invincibleEndTime = rollDuration * invincibleRatio;

        if (isInvincible && timer >= invincibleEndTime)
        {
            isInvincible = false;
            Debug.Log("<color=#fa0>翻滾：無敵幀結束（後段可受傷）</color>");
        }

        // ── 翻滾結束判斷 ─────────────────────────────
        if (timer >= rollDuration)
        {
            // 有排隊的下一次翻滾，直接重新進入翻滾狀態
            if (nextRollQueued && player.CanJump())
            {
                // 重新進入同一個狀態（連續翻滾）
                stateMachine.SwitchState(player.roll);
                return;
            }

            isRolling = false;
            stateMachine.SwitchState(player.idle);
        }
    }
}