using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// 玩家待機
/// </summary>
public class PlayerIdle : PlayerGround
{
    public PlayerIdle(StateMachine stateMachine, Player player, string name) : base(stateMachine, player, name)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //暫時註解掉，等之後有動畫再打開
        //player.ani.SetFloat(player.parHorizontal, 0);   //設定動畫參數 水平 為 0
        //player.ani.SetFloat(player.parVertical, 0);     //設定動畫參數 垂直 為 0
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        #region 條件區域
        //如果玩家的水平輸入 或者 垂直輸入 不等於 零 就切換到 走路狀態
        if (inputH != 0 || inputV != 0) stateMachine.SwitchState(player.walk);

        //跳躍
        //player.Jump();
        #endregion

    }
}
