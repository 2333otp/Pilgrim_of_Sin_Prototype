using UnityEngine;

/// <summary>
/// 玩家死亡
/// </summary>
public class PlayerDead : PlayerState
{
    public PlayerDead(StateMachine stateMachine, Player player, string name) : base(stateMachine, player, name)
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
    }
}