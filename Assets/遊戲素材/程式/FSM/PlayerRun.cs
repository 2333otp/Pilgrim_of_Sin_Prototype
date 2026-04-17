using UnityEngine;

/// <summary>
/// 玩家跑步
/// </summary>
public class PlayerRun : PlayerGround
{
    public PlayerRun(StateMachine stateMachine, Player player, string name) : base(stateMachine, player, name)
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

        player.ani.SetFloat(player.parHorizontal, inputH * 2);   //設定動畫參數 水平 為 水平輸入
        player.ani.SetFloat(player.parVertical, inputV * 2);     //設定動畫參數 垂直 為 垂直輸入

        //根據角色的方向設定加速度
        player.SetVelocity(
            player.transform.right * inputH * player.runSpeed +
            player.transform.forward * inputV * player.runSpeed +
            player.transform.up * player.rig.linearVelocity.y);

        //面向攝影機
        player.LookAtCamera();

        #region 條件區域
        //如果玩家的水平輸入 等於 零 並且 垂直輸入 等於 零 就切換到 待機狀態
        if (inputH == 0 && inputV == 0) stateMachine.SwitchState(player.idle);

        //如果玩家按 左邊 Shift 鍵 就切換到 跑步狀態
        if (Input.GetKeyUp(KeyCode.LeftShift)) stateMachine.SwitchState(player.walk);
        #endregion
    }
}

