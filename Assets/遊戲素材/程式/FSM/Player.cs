using System;
using UnityEngine;

/// <summary>
/// 玩家類別 : 紀錄玩家資料與相關功能
/// </summary>
public class Player : Character
{
    private static Player _instance;

    public static Player instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<Player>();
            return _instance;
        }
    }

    public event Action onDead;

    #region 玩家資料
    //唯獨屬性 : 讓外部取得此資料窗口但不能修改
    //序列化 : 讓私有欄位可以在編輯器中顯示與修改
    [field: Header("玩家資料")]
    //[field: SerializeField, Range(0,10)]
    //public float walkSpeed { get; private set; } = 2.5f;
    [field: SerializeField, Range(3, 15)]
    public float runSpeed { get; private set; } = 5;
    [field: SerializeField, Range(0, 20)]
    public float jumpHeight { get; private set; } = 7.5f;
    [field: SerializeField, Range(0, 30)]
    public float turnSpeed { get; private set; } = 15f;
    [field: SerializeField, Range(0, 3), Tooltip("中斷攻擊連段的時間")]
    public float breakComboTime { get; private set; } = 1f;

    //public Animator ani { get; private set; }
    //public Rigidbody rig { get; private set; }

    //public string parHorizontal { get; private set; } = "水平";
    //public string parVertical { get; private set; } = "垂直";
    //public string parTriggerAttack { get; private set; } = "觸發攻擊";
    //public string parTriggerDead { get; private set; } = "觸發死亡";

    public string parGravity { get; private set; } = "重力";
    public string parJump { get; private set; } = "開關跳躍";
    public string parAttackCombo { get; private set; } = "攻擊段數";
    public WeaponType currentWeapon { get; private set; } = WeaponType.Pencil;


    private Transform mainCam;
    #endregion

    #region 狀態資料
    //public StateMachine stateMachine { get; private set; }
    public PlayerIdle idle { get; private set; }
    public PlayerWalk walk { get; private set; }
    public PlayerRun run { get; private set; }
    public PlayerJump jump { get; private set; }
    public PlayerFall fall { get; private set; }
    public PlayerAttack attack { get; private set; }
    public PlayerDead dead { get; private set; }
    public PlayerWeaponSwitch weaponSwitch { get; private set; }
    public PlayerSpecialAttack specialAttack { get; private set; }
    public PlayerRoll roll { get; private set; }
    public LockOnSystem lockOn { get; private set; }

    #endregion

    #region 檢查資料
    [Header("檢查資料")]
    [SerializeField, Range(0, 1)]
    private float checkGroundRadius = 0.2f;          //檢查地板半徑
    [SerializeField, Range(-2, 2)]
    private float checkGroundOffsetY;                //檢查地板 Y 軸位移
    [SerializeField]
    private LayerMask layerCanJump;                  //可跳躍的圖層
    #endregion

    //ODGS 選取後繪製圖示
    private void OnDrawGizmosSelected()
    {
        //決定顏色
        Gizmos.color = new Color(0.5f, 1, 0.5f, 0.5f);
        //繪製球體
        Gizmos.DrawSphere(
            transform.position + new Vector3(0, checkGroundOffsetY, 0),
            checkGroundRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        //如果碰到物件 嘗試取得敵人攻擊物件 有資料 就造成傷害
       // if (other.TryGetComponent(out AttackObjectEnemy attackObject))
       // {
       //   Damage(attackObject.AttackPower);
       // }
    }

    protected override void Awake()
    {
        base.Awake();
        HideMouse();                        //隱藏滑鼠

        //ani = GetComponent<Animator>();     //取得動畫元件
        //rig = GetComponent<Rigidbody>();    //取得剛體元件
        mainCam = Camera.main.transform;    //取得主攝影機的變形元件 (貼 MainCamera 標籤)


        #region 狀態實例化
        //實例化 new 該類別 : 讓此類別不用掛在物件上也可以在場景內執行
        stateMachine = new StateMachine();
        idle = new PlayerIdle(stateMachine, this, $"{name} 待機");
        walk = new PlayerWalk(stateMachine, this, $"{name} 走路");
        run = new PlayerRun(stateMachine, this, $"{name} 跑步");
        jump = new PlayerJump(stateMachine, this, $"{name} 跳躍");
        fall = new PlayerFall(stateMachine, this, $"{name} 落下");
        attack = new PlayerAttack(stateMachine, this, $"{name} 攻擊");
        dead = new PlayerDead(stateMachine, this, $"{name} 死亡");
        weaponSwitch = new PlayerWeaponSwitch(stateMachine, this, $"{name} 武器切換");
        specialAttack = new PlayerSpecialAttack(stateMachine, this, $"{name} 特殊招式");
        roll = new PlayerRoll(stateMachine, this, $"{name} 翻滾");
        lockOn = GetComponent<LockOnSystem>();

        #endregion

        //初始化狀態機 為 待機狀態
        stateMachine.Initialize(idle);
    }

    private void Update()
    {
        //狀態機更新
        stateMachine.Update();
        InputSpecialAttack();
        InputRoll();            // ← 新增，放在攻擊之前
        InputAttack();                     //呼叫輸入攻擊方法
        InputWeaponSwitch();
        InputLockOn();

        //Debug.Log(CanJump());
    }

    /// <summary>
    /// 設定加速度
    /// </summary>
    /// <param name="direction">加速度方向</param>
    public void SetVelocity(Vector3 direction)
    {
        //剛體 的 線性加速度 = 方向
        rig.linearVelocity = direction;
    }

    /// <summary>
    /// 隱藏滑鼠
    /// </summary>
    public void HideMouse()
    {
        //隱藏滑鼠並鎖定在遊戲視窗中心
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 能否跳躍 : 檢查是否碰到可跳躍圖層物件
    /// </summary>
    public bool CanJump()
    {
        //檢查是否在地面上
        return Physics.CheckSphere(
            transform.position + new Vector3(0, checkGroundOffsetY, 0),
            checkGroundRadius, layerCanJump);
    }

    /// <summary>
    /// 輸入攻擊按鍵並進入攻擊狀態
    /// </summary>
    private void InputAttack()
    {
        // 讀取輕攻擊輸入（滑鼠左鍵 / J 鍵）
        bool lightPressed = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J);
        // 讀取重攻擊輸入（滑鼠右鍵 / I 鍵）
        bool heavyPressed = Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.I);

        // 如果都沒按就跳出
        if (!lightPressed && !heavyPressed) return;

        //測試期間暫時移除
        // 如果不在地面上就跳出（之後空中攻擊可移除此限制）
        //if (!CanJump()) return;

        // 武器切換動畫播放中，不可攻擊 
        if (weaponSwitch.isSwitching)
        {
            Debug.Log("<color=#f66>切換武器中，無法攻擊</color>");
            return;
        }
        // 翻滾中不可攻擊
        if (roll.isRolling)
        {
            Debug.Log("<color=#f66>翻滾中，無法攻擊</color>");
            return;
        }

        // 特殊招式執行中，不可被普通攻擊打斷 
        if (specialAttack.isSpecialAttacking)
        {
            Debug.Log("<color=#f66>特殊招式執行中，無法攻擊</color>");
            return;
        }

        // 把按鍵類型傳入攻擊狀態
        if (lightPressed)
            attack.ReceiveInput(AttackInputType.Light);
        else if (heavyPressed)
            attack.ReceiveInput(AttackInputType.Heavy);

        // 如果目前不在攻擊狀態，切換進入
        if (!attack.isAttacking)
            stateMachine.SwitchState(attack);

    }

    protected override void Damage(float damage)
    {
        base.Damage(damage);
        //CameraShake.instance.ShackCamera(0.2f, 5, 10f);
        StartCoroutine(DamageEffect(0.5f, 0.2f));
    }

    protected override void Dead()
    {
        base.Dead();
        //死亡後將物件圖層改為預設值
        gameObject.layer = 0;
        //呼叫死亡事件(? 指的是如果有訂閱的話才會呼叫)
        onDead?.Invoke();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //GameFlowManager.instance.ShowFinish("挑戰失敗!!!");
    }

    /// <summary>
    /// 設定目前武器（由 PlayerWeaponSwitch 呼叫）
    /// </summary>
    public void SetWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;
        Debug.Log($"<color=#9ff>武器切換為：{weapon}</color>");
    }

    /// <summary>
    /// 輸入武器切換按鍵
    /// </summary>
    private void InputWeaponSwitch()
    {
        WeaponType? target = null;

        // 鍵盤 1234 對應四種武器
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            target = WeaponType.Pencil;
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            target = WeaponType.Watercolor;
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            target = WeaponType.Knife;
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            target = WeaponType.Palette;

        if (target == null) return;

        // 嘗試切換，成功才進入切換狀態
        if (weaponSwitch.TrySwitch(target.Value))
            stateMachine.SwitchState(weaponSwitch);
    }

    /// <summary>
    /// 輸入特殊招式按鍵
    /// 優先度最高：可打斷普通攻擊與連段，但不可在特殊招式中再次觸發
    /// </summary>
    private void InputSpecialAttack()
    {
        // 讀取特殊招式按鍵（O 鍵 / 滑鼠中鍵）
        bool specialPressed = Input.GetKeyDown(KeyCode.O) ||
                              Input.GetKeyDown(KeyCode.Mouse2);

        if (!specialPressed) return;

        // 正在執行特殊招式中，不可打斷自己
        if (specialAttack.isSpecialAttacking)
        {
            Debug.Log("<color=#f66>特殊招式執行中，無法再次觸發</color>");
            return;
        }

        // 武器切換中，不可使用特殊招式
        if (weaponSwitch.isSwitching)
        {
            Debug.Log("<color=#f66>切換武器中，無法使用特殊招式</color>");
            return;
        }

        // 檢查 CD
        if (!specialAttack.CanUse())
        {
            Debug.Log($"<color=#f66>特殊招式冷卻中，剩餘 " +
                      $"{specialAttack.CooldownRemaining():F1} 秒</color>");
            return;
        }

        // 通過所有檢查，切換到特殊招式狀態
        // 注意：此處不需要檢查 attack.isAttacking
        // 因為特殊招式優先度最高，可以直接打斷普通攻擊與連段
        Debug.Log("<color=#f0f>特殊招式輸入通過，切換狀態</color>");
        stateMachine.SwitchState(specialAttack);
    }

    /// <summary>
    /// 面向移動方向（取代 LookAtCamera，移動時使用）
    /// </summary>
    /// <param name="moveDir">移動方向向量</param>
    public void LookAtMoveDirection(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.001f) return;  // 向量太小就不旋轉

        // 目標角度：面向移動方向
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        // 插值平滑旋轉
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 鎖定時面向目標（取代 LookAtMoveDirection）
    /// </summary>
    public void LookAtTarget()
    {
        if (lockOn == null || !lockOn.isLockedOn || lockOn.lockedTarget == null) return;

        Vector3 dirToTarget = lockOn.lockedTarget.position - transform.position;
        dirToTarget.y = 0f;
        if (dirToTarget.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 輸入翻滾按鍵（Shift）
    /// 優先度：低於特殊招式，高於普通攻擊
    /// </summary>
    /// 
    private void InputRoll()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift)) return;

        // 特殊招式執行中，不可翻滾（特殊招式優先度最高）
        if (specialAttack.isSpecialAttacking)
        {
            Debug.Log("<color=#f66>特殊招式執行中，無法翻滾</color>");
            return;
        }

        // 武器切換中，不可翻滾
        if (weaponSwitch.isSwitching)
        {
            Debug.Log("<color=#f66>切換武器中，無法翻滾</color>");
            return;
        }

        // 不在地面上，不可翻滾
        if (!CanJump())
        {
            Debug.Log("<color=#f66>空中無法翻滾</color>");
            return;
        }

        // 正在翻滾中：排隊下一次翻滾（連續施放）
        if (roll.isRolling)
        {
            roll.QueueNextRoll();
            return;
        }

        // 通過所有檢查，進入翻滾狀態
        stateMachine.SwitchState(roll);
    }

    /// <summary>
    /// 輸入鎖定按鍵
    /// </summary>
    private void InputLockOn()
    {
        // Q 鍵切換鎖定
        if (Input.GetKeyDown(KeyCode.Q))
            lockOn.ToggleLockOn();

        // P 鍵切換鎖定目標
        if (Input.GetKeyDown(KeyCode.P))
            lockOn.SwitchTarget();
    }
}