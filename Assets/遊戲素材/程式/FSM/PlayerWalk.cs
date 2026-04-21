using UnityEngine;

/// <summary>
/// 玩家走路
/// 八方向移動（相對攝影機）
/// 長按 1.2 秒自動切換為跑步
/// </summary>
public class PlayerWalk : PlayerGround
{
    // 長按切跑的時間門檻（文件：1.2 秒）
    private float runThreshold = 1.2f;
    // 累積按住移動鍵的時間
    private float holdTimer = 0f;

    public PlayerWalk(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 進入走路狀態時重置長按計時器
        holdTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 計算八方向移動向量（相對攝影機）
        Vector3 moveDir = GetCameraMoveDirection();

        // 套用走路速度
        player.SetVelocity(
            moveDir * player.walkSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        // 有輸入才累加長按計時器
        if (moveDir.magnitude > 0f)
        {
            holdTimer += Time.deltaTime;

            // 鎖定中面向目標，否則面向移動方向
            if (player.lockOn != null && player.lockOn.isLockedOn)
                player.LookAtTarget();
            else
                player.LookAtMoveDirection(moveDir);
        }

        // 動畫參數（有動畫後取消註解）
        // player.ani.SetFloat(player.parHorizontal, inputH);
        // player.ani.SetFloat(player.parVertical, inputV);

        #region 條件區域
        // 沒有輸入 → 待機
        if (inputH == 0 && inputV == 0)
            stateMachine.SwitchState(player.idle);

        // 長按超過門檻 → 跑步
        if (holdTimer >= runThreshold)
            stateMachine.SwitchState(player.run);
        #endregion
    }

    /// <summary>
    /// 根據攝影機方向計算八方向移動向量
    /// 斜角已正規化，確保斜角速度與直角相同
    /// </summary>
    private Vector3 GetCameraMoveDirection()
    {
        // 取得攝影機的前方與右方（忽略 Y 軸，只取水平分量）
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 組合輸入方向
        Vector3 moveDir = camForward * inputV + camRight * inputH;

        // 正規化：確保斜角（例如 W+D）速度與直角（只按 W）相同
        // 不超過長度 1
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        return moveDir;
    }
}