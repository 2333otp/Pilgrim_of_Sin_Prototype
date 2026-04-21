using UnityEngine;

/// <summary>
/// 攝影機旋轉控制
/// 一般狀態：滑鼠控制
/// 鎖定狀態：完全面向鎖定目標
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("旋轉速度")]
    [SerializeField, Range(0, 500)]
    private float horizontalSpeed = 200f;
    [SerializeField, Range(0, 500)]
    private float verticalSpeed = 150f;

    [Header("上下角度限制")]
    [SerializeField, Range(-90, 0)]
    private float minVerticalAngle = -20f;
    [SerializeField, Range(0, 90)]
    private float maxVerticalAngle = 40f;

    [Header("設定")]
    [SerializeField]
    private bool invertY = false;

    [Header("鎖定設定")]
    [SerializeField, Range(1, 20)]
    private float lockOnRotateSpeed = 5f;   // 鎖定時攝影機轉向速度

    private float currentYaw;
    private float currentPitch;
    private LockOnSystem lockOn;

    private void Start()
    {
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;

        // 取得 LockOnSystem（在 Player 物件上）
        lockOn = FindAnyObjectByType<LockOnSystem>();
    }

    private void Update()
    {
        if (lockOn != null && lockOn.isLockedOn)
            UpdateLockOnRotation();
        else
            UpdateFreeRotation();
    }

    /// <summary>
    /// 一般狀態：滑鼠控制旋轉
    /// </summary>
    private void UpdateFreeRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (invertY) mouseY = -mouseY;

        currentYaw += mouseX * horizontalSpeed * Time.deltaTime;
        currentPitch -= mouseY * verticalSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
    }

    /// <summary>
    /// 鎖定狀態：攝影機平滑轉向鎖定目標
    /// </summary>
    private void UpdateLockOnRotation()
    {
        Vector3 dirToTarget = lockOn.GetDirectionToTarget();
        if (dirToTarget == Vector3.zero) return;

        // 計算目標旋轉角度
        Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);

        // 平滑插值轉向目標
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            lockOnRotateSpeed * Time.deltaTime);

        // 同步 currentYaw 和 currentPitch
        // 避免解除鎖定時攝影機瞬間跳回去
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
    }
}
