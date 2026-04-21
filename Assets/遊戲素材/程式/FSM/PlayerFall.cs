using UnityEngine;

/// <summary>
/// 玩家落下
/// 規則：
///   - 繼承跳躍的鎖定方向，滯空期間不可飄移不可旋轉
///   - 落地後切回待機
///   - 滯空中可再次按空白鍵連跳
/// </summary>
public class PlayerFall : PlayerState
{
    public PlayerFall(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // player.ani.SetFloat(player.parGravity, -1);  // 有動畫後取消註解
    }

    public override void Exit()
    {
        base.Exit();
        // player.ani.SetBool(player.parJump, false);   // 有動畫後取消註解
    }

    public override void Update()
    {
        base.Update();

        // 計算八方向移動向量（相對攝影機）
        Vector3 moveDir = GetCameraMoveDirection();

        // Fall 狀態不修改水平速度
        // 保留 Jump 離開時的水平慣性，只讓重力作用在 Y 軸
        // 不呼叫 SetVelocity 修改水平，避免飄移
        player.SetVelocity(
            new Vector3(
                player.rig.linearVelocity.x,    // 保持原本水平速度不變
                player.rig.linearVelocity.y,    // Y 軸繼續受重力
                player.rig.linearVelocity.z));

        // 有輸入才面向移動方向
        if (moveDir.magnitude > 0f)
        {
            // 鎖定中面向目標，否則面向移動方向
            if (player.lockOn != null && player.lockOn.isLockedOn)
                player.LookAtTarget();
            else
                player.LookAtMoveDirection(moveDir);
        }
        // 禁止旋轉：不呼叫 LookAtMoveDirection

        #region 條件區域
        if (player.CanJump())
        {
            // 落地時清除水平速度，避免殘留速度造成旋轉
            player.SetVelocity(new Vector3(0, player.rig.linearVelocity.y, 0));
            stateMachine.SwitchState(player.idle);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.SwitchState(player.jump);
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