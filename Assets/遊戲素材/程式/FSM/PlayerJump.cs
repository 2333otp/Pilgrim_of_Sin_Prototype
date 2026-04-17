using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// 玩家跳躍
/// </summary>
public class PlayerJump : PlayerState
{
    public PlayerJump(StateMachine stateMachine, Player player, string name) : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //添加向上的加速度
        player.SetVelocity(player.transform.up * player.jumpHeight);
        //勾選開關跳躍
        player.ani.SetBool(player.parJump, true);
        //設定重力浮點數為 1
        player.ani.SetFloat(player.parGravity, 1);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //根據角色的方向設定加速度
        player.SetVelocity(
            player.transform.right * inputH * player.walkSpeed +
            player.transform.forward * inputV * player.walkSpeed +
            player.transform.up * player.rig.linearVelocity.y);

        //面向攝影機
        player.LookAtCamera();

        #region 條件區域
        //如果 玩家的重力 小於 0 就切換到 落下狀態
        if (player.rig.linearVelocity.y < 0)
            stateMachine.SwitchState(player.fall);
        #endregion
    }
}