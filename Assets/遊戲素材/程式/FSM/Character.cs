using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 角色：角色基本資料與功能
/// abstract 抽象類別：不會有實體
/// </summary>
public abstract class Character : MonoBehaviour
{
    [field: Header("角色資料")]
    [field: SerializeField, Range(0, 10)]
    public float walkSpeed { get; private set; } = 2.5f;
    [SerializeField, Range(0, 500)]
    protected float hpMax = 100;

    protected float hp;

    public Animator ani { get; private set; }
    public Rigidbody rig { get; private set; }
    public string parHorizontal { get; private set; } = "水平";
    public string parVertical { get; private set; } = "垂直";
    public string parTriggerAttack { get; private set; } = "觸發攻擊";
    public string parTriggerDead { get; private set; } = "觸發死亡";

    public StateMachine stateMachine { get; protected set; }

    [SerializeField]
    protected Image imgHp;
    [SerializeField]
    protected TMP_Text textHp;
    [SerializeField]
    protected AudioClip soundHurt, soundDeath;

    protected virtual void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        hp = hpMax;

        if (textHp != null)
            textHp.text = $"{hp} / {hpMax}";
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">受傷值</param>
    protected virtual void Damage(float damage)
    {
        if (hp <= 0) return;

        hp -= damage;
        hp = Mathf.Clamp(hp, 0, hpMax);

        if (imgHp != null)
            imgHp.fillAmount = hp / hpMax;
        if (textHp != null)
            textHp.text = $"{hp} / {hpMax}";

        if (hp <= 0) Dead();
        Debug.Log($"<color=#66f>{gameObject.name} 剩餘血量 {hp}</color>");
        //SoundManager.instance.PlaySound(soundHurt, 0.7f, 1.3f);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Dead()
    {
        ani.SetTrigger(parTriggerDead);
        rig.isKinematic = true;
        enabled = false;
        //SoundManager.instance.PlaySound(soundDeath, 0.7f, 1.3f);
    }

    /// <summary>
    /// 受傷效果：卡肉感
    /// </summary>
    protected IEnumerator DamageEffect(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sound">音效檔</param>
    protected void PlaySound(AudioClip sound)
    {
        //SoundManager.instance.PlaySound(sound, 0.8f, 1.2f);
    }
}