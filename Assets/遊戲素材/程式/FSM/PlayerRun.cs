using UnityEngine;

/// <summary>
/// 玩家跑步
/// 八方向移動（相對攝影機）
/// 放開移動鍵 → 切回走路
/// </summary>
public class PlayerRun : PlayerGround
{
    public PlayerRun(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Vector3 moveDir = (player.lockOn != null && player.lockOn.isLockedOn)
            ? player.GetLockOnMoveDirection()
            : GetCameraMoveDirection();

        player.SetVelocity(
            moveDir * player.runSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        if (moveDir.magnitude > 0f)
        {
            if (player.lockOn != null && player.lockOn.isLockedOn)
                player.LookAtTarget();
            else
                player.LookAtMoveDirection(moveDir);
        }

        // 動畫參數（有動畫後取消註解）
        // player.ani.SetFloat(player.parHorizontal, inputH * 2);
        // player.ani.SetFloat(player.parVertical, inputV * 2);

        #region 條件區域
        // 沒有輸入 → 待機
        if (inputH == 0 && inputV == 0)
            stateMachine.SwitchState(player.idle);
        #endregion
    }

    /// <summary>
    /// 根據攝影機方向計算八方向移動向量
    /// 與 PlayerWalk 相同邏輯，斜角已正規化
    /// </summary>
    private Vector3 GetCameraMoveDirection()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputV + camRight * inputH;

        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        return moveDir;
    }
}

