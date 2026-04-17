using UnityEngine;

/// <summary>
/// 武器切換狀態
/// 進入此狀態後等待 switchDuration 秒（模擬動畫時間），
/// 結束後自動回到 Idle
/// </summary>
public class PlayerWeaponSwitch : PlayerState
{
    // 模擬切換動畫的持續時間（有動畫後改為讀取動畫長度）
    private float switchDuration = 0.5f;

    // 切換冷卻：切回同一把武器需要等待的時間
    private float switchCooldown = 1.0f;

    // 記錄每把武器上次切換離開的時間
    private float[] lastSwitchTime;

    // 是否正在切換中（切換動畫播放期間）
    public bool isSwitching { get; private set; }

    public PlayerWeaponSwitch(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
        // 四種武器各自記錄冷卻時間，初始為負數確保一開始都可以切換
        lastSwitchTime = new float[4] { -999f, -999f, -999f, -999f };
    }

    /// <summary>
    /// 嘗試切換到指定武器，回傳是否成功
    /// 由 Player.InputWeaponSwitch() 呼叫
    /// </summary>
    public bool TrySwitch(WeaponType targetWeapon)
    {
        int index = (int)targetWeapon;

        // 正在切換中，不可切換
        if (isSwitching)
        {
            Debug.Log("<color=#f66>切換中，無法再切換</color>");
            return false;
        }

        // 目標武器與目前武器相同，檢查 CD
        if (targetWeapon == player.currentWeapon)
        {
            Debug.Log($"<color=#f66>{targetWeapon} 已是目前武器</color>");
            return false;
        }

        // 檢查目標武器的冷卻時間
        // （切換「離開」某武器時記錄時間，切回來時檢查是否過了 CD）
        float timeSinceLeft = Time.time - lastSwitchTime[index];
        if (timeSinceLeft < switchCooldown)
        {
            float remaining = switchCooldown - timeSinceLeft;
            Debug.Log($"<color=#f66>{targetWeapon} 冷卻中，剩餘 {remaining:F1} 秒</color>");
            return false;
        }

        // 記錄「離開目前武器」的時間（為了之後切回來時計算 CD）
        lastSwitchTime[(int)player.currentWeapon] = Time.time;

        // 切換武器
        player.SetWeapon(targetWeapon);
        return true;
    }

    public override void Enter()
    {
        base.Enter();
        isSwitching = true;
        Debug.Log($"<color=#9ff>開始切換武器 → {player.currentWeapon}（模擬動畫 {switchDuration} 秒）</color>");
    }

    public override void Exit()
    {
        base.Exit();
        isSwitching = false;
        Debug.Log("<color=#9ff>武器切換完成</color>");
    }

    public override void Update()
    {
        base.Update();

        // 等待模擬動畫時間結束後回到待機
        // 有動畫後改為：timer >= player.ani.GetCurrentAnimatorStateInfo(0).length
        if (timer >= switchDuration)
            stateMachine.SwitchState(player.idle);
    }
}