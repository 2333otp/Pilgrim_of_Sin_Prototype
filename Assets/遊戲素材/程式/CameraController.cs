using UnityEngine;

/// <summary>
/// 攝影機旋轉控制
/// 掛在 CameraTarget 上，Third Person Follow 跟隨此物件旋轉
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("旋轉速度")]
    [SerializeField, Range(0, 500)]
    private float horizontalSpeed = 200f;   // 左右旋轉速度
    [SerializeField, Range(0, 500)]
    private float verticalSpeed = 150f;     // 上下旋轉速度

    [Header("上下角度限制")]
    [SerializeField, Range(-90, 0)]
    private float minVerticalAngle = -20f;  // 向下最大角度
    [SerializeField, Range(0, 90)]
    private float maxVerticalAngle = 40f;   // 向上最大角度

    [Header("設定")]
    [SerializeField]
    private bool invertY = false;           // 是否反轉上下軸

    private float currentYaw;    // 目前水平角度（左右）
    private float currentPitch;  // 目前垂直角度（上下）

    private void Start()
    {
        // 初始化角度為目前旋轉值
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        // 讀取滑鼠輸入
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 反轉 Y 軸（可在 Inspector 調整）
        if (invertY) mouseY = -mouseY;

        // 累加角度
        currentYaw += mouseX * horizontalSpeed * Time.deltaTime;
        currentPitch -= mouseY * verticalSpeed * Time.deltaTime;

        // 限制上下角度範圍
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // 套用旋轉
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);

        // CameraTarget 跟隨 Player 位置（Parent 是 Player 所以位置自動跟隨）
    }
}