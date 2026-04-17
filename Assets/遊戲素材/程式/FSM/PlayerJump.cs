using UnityEngine;

/// <summary>
/// 玩家跳躍
/// 規則：
///   - 起跳時鎖定角色當下面對的方向
///   - 滯空期間不可旋轉、不可飄移
///   - 可被敵人打斷
/// </summary>
public class PlayerJump : PlayerState
{
    // 起跳時鎖定的水平移動向量（滯空期間固定不變）
    private Vector3 lockedMoveDir;

    public PlayerJump(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 起跳瞬間：鎖定目前角色面對的方向作為水平移動方向
        // transform.forward = 角色當下面對的方向（已在移動時轉好了）
        lockedMoveDir = player.transform.forward;

        // 施加向上的力
        player.SetVelocity(
            lockedMoveDir * player.walkSpeed +
            Vector3.up * player.jumpHeight);

        // 動畫（有動畫後取消註解）
        // player.ani.SetBool(player.parJump, true);
        // player.ani.SetFloat(player.parGravity, 1);

        Debug.Log($"<color=#6ff>起跳 | 鎖定方向：{lockedMoveDir}</color>");
    }

    public override void Exit()
    {
        base.Exit();
        // player.ani.SetBool(player.parJump, false);  // 有動畫後取消註解
    }

    public override void Update()
    {
        base.Update();

        // 滯空期間：水平方向固定為起跳時的 lockedMoveDir，不受輸入影響
        // 垂直方向繼續受重力影響（保留 Y 軸速度）
        player.SetVelocity(
            lockedMoveDir * player.walkSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        // 禁止旋轉：不呼叫 LookAtMoveDirection，角色方向維持起跳時的朝向

        #region 條件區域
        if (player.rig.linearVelocity.y < 0)
        {
            // 進入落下狀態前保留水平速度但不做額外處理
            stateMachine.SwitchState(player.fall);
        }
        #endregion
    }
}