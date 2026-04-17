using UnityEngine;

/// <summary>
/// 狀態機
/// </summary>
public class StateMachine
{
    //紀錄目前狀態
    private State currentState;

    //功能 = 方法 = 函式 = 函數 | method = function
    /// <summary>
    /// 初始化狀態機
    /// </summary>
    /// <param name="firstState">第一狀態</param>
    public void Initialize(State firstState)
    {
        currentState = firstState;
        currentState.Enter();
    }

    /// <summary>
    /// 切換狀態:先退出原本狀態，再進入新狀態
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchState(State newState)
    {
        //離開目前狀態
        currentState.Exit();
        //指定新狀態為目前狀態
        currentState = newState;
        //進入目前狀態
        currentState.Enter();
    }

    /// <summary>
    /// 更新狀態:持續執行目前狀態
    /// </summary>
    public void Update()
    {
        //更新目前狀態
        currentState.Update();
    }

}
