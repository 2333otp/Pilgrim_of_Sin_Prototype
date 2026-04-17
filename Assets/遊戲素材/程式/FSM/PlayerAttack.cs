using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家攻擊
/// 負責：接收攻擊輸入、記錄按鍵序列、比對連段、輸出 Debug 訊息
/// </summary>
public class PlayerAttack : PlayerState
{
    // ────────────────────────────────────────
    // 連段資料（四種武器）
    // ────────────────────────────────────────

    // 以武器類型為 key，存放各武器的連段列表
    private Dictionary<WeaponType, List<ComboData>> allCombos;

    // ────────────────────────────────────────
    // 連段狀態追蹤
    // ────────────────────────────────────────

    private List<AttackInputType> inputSequence;  // 目前已輸入的按鍵序列
    private float lastInputTime;                  // 上次輸入的時間
    private float inputWindowTime = 0.8f;         // 輸入窗口（秒），超時就重置序列

    // ────────────────────────────────────────
    // 攻擊狀態旗標
    // ────────────────────────────────────────

    public bool isAttacking { get; private set; }
    private float attackFinishTime;
    private string currentComboName;              // 目前觸發的連段名稱（Debug 用）

    // ────────────────────────────────────────
    // 建構函式
    // ────────────────────────────────────────

    public PlayerAttack(StateMachine stateMachine, Player player, string name)
        : base(stateMachine, player, name)
    {
        inputSequence = new List<AttackInputType>();

        // ── 連段定義說明 ──────────────────────────────────
        // 序列越長的連段排在越前面
        // 讓比對時優先命中長連段，避免短連段提早觸發
        // 對照文件武器連段：
        //
        // 鉛筆：Combo1 輕重 / Combo2 重重 / Combo3 重輕 / Combo4 輕輕輕重
        // 水彩筆：Combo1 輕重 / Combo2 輕輕重 / Combo3 重重輕 / Combo4 重輕重
        // 畫刀：Combo1 輕重重 / Combo2 輕輕重重 / Combo3 重輕重輕 / Combo4 輕重重重
        // 調色盤：Combo1 輕重輕重 / Combo2 輕重輕 / Combo3 重重重 / Combo4 重（下押前）

        allCombos = new Dictionary<WeaponType, List<ComboData>>
        {
            // ── 鉛筆 ──────────────────────────────────────
            {
                WeaponType.Pencil, new List<ComboData>
                {
                    new ComboData(WeaponType.Pencil, "鉛筆_Combo4_輕輕輕重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Light,
                            AttackInputType.Light, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Pencil, "鉛筆_Combo1_輕重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Pencil, "鉛筆_Combo2_重重",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Pencil, "鉛筆_Combo3_重輕",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Light }),
                }
            },

            // ── 水彩筆 ────────────────────────────────────
            // 注意：Combo3 的文件寫「下押前，重重輕」
            // 「下押前」為方向輸入，目前階段先略過方向判斷，只記錄重重輕
            {
                WeaponType.Watercolor, new List<ComboData>
                {
                    new ComboData(WeaponType.Watercolor, "水彩筆_Combo2_輕輕重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Light,
                            AttackInputType.Heavy }),

                    new ComboData(WeaponType.Watercolor, "水彩筆_Combo3_重重輕(下押前)",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Heavy,
                            AttackInputType.Light }),

                    new ComboData(WeaponType.Watercolor, "水彩筆_Combo4_重輕重",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Light,
                            AttackInputType.Heavy }),

                    new ComboData(WeaponType.Watercolor, "水彩筆_Combo1_輕重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy }),
                }
            },

            // ── 畫刀 ──────────────────────────────────────
            {
                WeaponType.Knife, new List<ComboData>
                {
                    new ComboData(WeaponType.Knife, "畫刀_Combo2_輕輕重重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Light,
                            AttackInputType.Heavy, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Knife, "畫刀_Combo4_輕重重重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy,
                            AttackInputType.Heavy, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Knife, "畫刀_Combo3_重輕重輕",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Light,
                            AttackInputType.Heavy, AttackInputType.Light }),

                    new ComboData(WeaponType.Knife, "畫刀_Combo1_輕重重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy,
                            AttackInputType.Heavy }),
                }
            },

            // ── 調色盤 ────────────────────────────────────
            // 注意：Combo4 文件寫「下押前，重」
            // 方向輸入目前階段略過，只記錄重
            {
                WeaponType.Palette, new List<ComboData>
                {
                    new ComboData(WeaponType.Palette, "調色盤_Combo1_輕重輕重",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy,
                            AttackInputType.Light, AttackInputType.Heavy }),

                    new ComboData(WeaponType.Palette, "調色盤_Combo2_輕重輕",
                        new List<AttackInputType> {
                            AttackInputType.Light, AttackInputType.Heavy,
                            AttackInputType.Light }),

                    new ComboData(WeaponType.Palette, "調色盤_Combo3_重重重",
                        new List<AttackInputType> {
                            AttackInputType.Heavy, AttackInputType.Heavy,
                            AttackInputType.Heavy }),
                }
            },
        };
    }

    // ────────────────────────────────────────
    // 接收外部輸入（由 Player.InputAttack() 呼叫）
    // ────────────────────────────────────────

    /// <summary>
    /// 接收一次攻擊輸入，加入序列並嘗試比對連段
    /// </summary>
    public void ReceiveInput(AttackInputType inputType)
    {
        float now = Time.time;

        // 距離上次輸入超過輸入窗口，重置序列
        if (now - lastInputTime > inputWindowTime)
        {
            inputSequence.Clear();
            Debug.Log("<color=#aaa>序列逾時重置</color>");
        }

        inputSequence.Add(inputType);
        lastInputTime = now;

        Debug.Log($"<color=#ff9>[{player.currentWeapon}] 目前序列 : {SequenceToString(inputSequence)}</color>");

        TryMatchCombo();
    }

    // ────────────────────────────────────────
    // 連段比對
    // ────────────────────────────────────────

    private void TryMatchCombo()
    {
        // 取得目前武器的連段列表
        if (!allCombos.TryGetValue(player.currentWeapon, out List<ComboData> combos))
        {
            Debug.LogWarning($"找不到武器 {player.currentWeapon} 的連段資料");
            return;
        }

        foreach (ComboData combo in combos)
        {
            int comboLen = combo.sequence.Count;
            if (inputSequence.Count < comboLen) continue;

            bool matched = true;
            for (int i = 0; i < comboLen; i++)
            {
                int seqIndex = inputSequence.Count - comboLen + i;
                if (inputSequence[seqIndex] != combo.sequence[i])
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
            {
                currentComboName = combo.comboName;
                inputSequence.Clear();
                Debug.Log($"<color=#0f9>✔ 觸發連段 : {currentComboName}</color>");
                return;
            }
        }

        // 沒比對到連段，輸出普通攻擊
        AttackInputType lastInput = inputSequence[inputSequence.Count - 1];
        string attackType = lastInput == AttackInputType.Light ? "輕攻擊" : "重攻擊";
        Debug.Log($"<color=#6cf>[{player.currentWeapon}] 普通{attackType}</color>");
    }

    // ────────────────────────────────────────
    // 狀態機標準方法
    // ────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();
        isAttacking = true;
        // player.ani.applyRootMotion = true;          // 有動畫後取消註解
        // player.ani.SetTrigger(player.parTriggerAttack); // 有動畫後取消註解
        Debug.Log($"<color=#ff3>進入攻擊狀態 | 武器：{player.currentWeapon}</color>");
    }

    public override void Exit()
    {
        base.Exit();
        isAttacking = false;
        // player.ani.applyRootMotion = false;  // 有動畫後取消註解
        attackFinishTime = Time.time;

        // 離開攻擊狀態時清空序列，避免殘留輸入影響下一次攻擊
        inputSequence.Clear();

        Debug.Log("<color=#f66>離開攻擊狀態</color>");
    }

    public override void Update()
    {
        base.Update();

        // 攻擊中允許小幅移動（速度為走路的 40%）
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputV + camRight * inputH;
        if (moveDir.magnitude > 1f) moveDir.Normalize();

        float attackMoveSpeed = player.walkSpeed * 0.4f;
        player.SetVelocity(
            moveDir * attackMoveSpeed +
            Vector3.up * player.rig.linearVelocity.y);

        // 有動畫後改為讀取動畫長度
        if (timer >= player.breakComboTime)
        {
            isAttacking = false;
            stateMachine.SwitchState(player.idle);
        }
    }

    // ────────────────────────────────────────
    // 工具方法
    // ────────────────────────────────────────

    private string SequenceToString(List<AttackInputType> seq)
    {
        string result = "[";
        for (int i = 0; i < seq.Count; i++)
        {
            result += seq[i] == AttackInputType.Light ? "輕" : "重";
            if (i < seq.Count - 1) result += ", ";
        }
        result += "]";
        return result;
    }
}