using UnityEngine;

/// <summary>
/// 狀態
/// </summary>
public class State
{
    // protected 受保護:允許子類別存取
    protected string name; //玩家
    protected StateMachine stateMachine; //狀態機
    protected float timer; //計時器

    //virtual 虛擬:允許子類別覆寫此方法
    /// <summary>
    /// 進入狀態
    /// </summary>
    public virtual void Enter()
    {
        timer = 0f; // 進入狀態時重置計時器
    }

    /// <summary>
    /// 更新狀態
    /// </summary>
    public virtual void Update()
    {
        //累加時間到計時器內
        // Time.deltaTime 每個影格的時間，大約 0.02 秒 (1/60)
        timer += Time.deltaTime;
    }

    /// <summary>
    /// 離開狀態
    /// </summary>
    public virtual void Exit()
    {

    }
}

