using UnityEngine;

public class PlayerGround : PlayerState
{
    public PlayerGround(StateMachine stateMachine, Player player, string name) : base(stateMachine, player, name)
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

        #region 條件區域
        // 翻滾中不可跳躍
        if (player.CanJump() && Input.GetKeyDown(KeyCode.Space) && !player.roll.isRolling)
            stateMachine.SwitchState(player.jump);
        #endregion

        Debug.Log($"CanJump: {player.CanJump()} | Space: {Input.GetKeyDown(KeyCode.Space)}");

    }
}