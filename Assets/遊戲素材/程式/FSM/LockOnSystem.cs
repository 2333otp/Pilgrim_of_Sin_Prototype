using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鎖定系統
/// 規則：
///   - 按一次鎖定最近敵人，再按一次解除
///   - 鎖定切換依照距離由近到遠
///   - 鎖定時攝影機完全面對目標
///   - 敵人死亡自動順延下一個
/// </summary>
public class LockOnSystem : MonoBehaviour
{
    // ────────────────────────────────────────
    // 設定
    // ────────────────────────────────────────

    [Header("鎖定設定")]
    [SerializeField, Range(1, 30)]
    private float lockOnRange = 15f;        // 鎖定範圍（超出範圍自動解除）

    [SerializeField]
    private Transform cameraTarget;         // 拖入 CameraTarget 物件

    // ────────────────────────────────────────
    // 鎖定狀態
    // ────────────────────────────────────────

    public bool isLockedOn { get; private set; }    // 是否鎖定中
    public Transform lockedTarget { get; private set; }  // 目前鎖定目標

    // 已鎖定過的敵人列表（用於切換時循環）
    private List<Transform> lockedHistory = new List<Transform>();

    // ────────────────────────────────────────
    // Update
    // ────────────────────────────────────────

    private void Update()
    {
        // 鎖定中：持續確認目標是否還在範圍內
        if (isLockedOn)
        {
            CheckTargetValid();
        }
    }

    // ────────────────────────────────────────
    // 鎖定開關（由 Player.InputLockOn() 呼叫）
    // ────────────────────────────────────────

    /// <summary>
    /// 切換鎖定狀態
    /// </summary>
    public void ToggleLockOn()
    {
        if (isLockedOn)
        {
            // 目前鎖定中 → 解除
            Unlock();
        }
        else
        {
            // 目前未鎖定 → 鎖定最近敵人
            Transform nearest = FindNearestEnemy();
            if (nearest != null)
                LockOn(nearest);
            else
                Debug.Log("<color=#f66>鎖定：範圍內沒有敵人</color>");
        }
    }

    /// <summary>
    /// 切換鎖定目標（Q 切換，依距離順序循環）
    /// </summary>
    public void SwitchTarget()
    {
        if (!isLockedOn)
        {
            Debug.Log("<color=#f66>鎖定切換：尚未鎖定任何目標</color>");
            return;
        }

        // 取得所有敵人並依距離排序
        List<Transform> enemies = GetAllEnemiesSortedByDistance();

        if (enemies.Count <= 1)
        {
            Debug.Log("<color=#f66>鎖定切換：場上只有一個敵人</color>");
            return;
        }

        // 找到目前鎖定目標在排序後列表中的位置
        int currentIndex = enemies.IndexOf(lockedTarget);

        // 切換到下一個（循環）
        int nextIndex = (currentIndex + 1) % enemies.Count;
        LockOn(enemies[nextIndex]);

        Debug.Log($"<color=#ff9>鎖定切換 → {enemies[nextIndex].name}</color>");
    }

    // ────────────────────────────────────────
    // 內部方法
    // ────────────────────────────────────────

    /// <summary>
    /// 鎖定指定目標
    /// </summary>
    private void LockOn(Transform target)
    {
        lockedTarget = target;
        isLockedOn = true;
        Debug.Log($"<color=#0ff>鎖定：{target.name}</color>");
    }

    /// <summary>
    /// 解除鎖定
    /// </summary>
    private void Unlock()
    {
        Debug.Log($"<color=#aaa>解除鎖定：{lockedTarget?.name}</color>");
        lockedTarget = null;
        isLockedOn = false;
        lockedHistory.Clear();
    }

    /// <summary>
    /// 確認目前鎖定目標是否還有效
    /// 無效條件：目標被刪除、超出範圍、目標死亡
    /// </summary>
    private void CheckTargetValid()
    {
        // 目標物件已被刪除
        if (lockedTarget == null)
        {
            Debug.Log("<color=#aaa>鎖定目標消失，自動解除</color>");

            // 嘗試順延到下一個最近的敵人
            Transform next = FindNearestEnemy();
            if (next != null)
            {
                Debug.Log($"<color=#0ff>自動順延鎖定：{next.name}</color>");
                LockOn(next);
            }
            else
            {
                Unlock();
            }
            return;
        }

        // 目標超出鎖定範圍
        float dist = Vector3.Distance(transform.position, lockedTarget.position);
        if (dist > lockOnRange)
        {
            Debug.Log("<color=#aaa>鎖定目標超出範圍，自動解除</color>");
            Unlock();
        }
    }

    /// <summary>
    /// 找到範圍內最近的敵人
    /// </summary>
    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float nearestDist = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= lockOnRange && dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 取得所有範圍內敵人，依距離由近到遠排序
    /// </summary>
    private List<Transform> GetAllEnemiesSortedByDistance()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Transform> result = new List<Transform>();

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= lockOnRange)
                result.Add(enemy.transform);
        }

        // 依距離排序
        result.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.position);
            float distB = Vector3.Distance(transform.position, b.position);
            return distA.CompareTo(distB);
        });

        return result;
    }

    // ────────────────────────────────────────
    // 攝影機控制（鎖定時面向目標）
    // ────────────────────────────────────────

    /// <summary>
    /// 取得鎖定目標方向給 CameraController 使用
    /// </summary>
    public Vector3 GetDirectionToTarget()
    {
        if (!isLockedOn || lockedTarget == null)
            return Vector3.zero;

        return (lockedTarget.position - cameraTarget.position).normalized;
    }

    // ────────────────────────────────────────
    // Gizmos（Scene 視窗顯示鎖定範圍）
    // ────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        // 顯示鎖定範圍球體
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, lockOnRange);

        // 顯示鎖定連線
        if (isLockedOn && lockedTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, lockedTarget.position);
        }
    }
}