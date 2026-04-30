/// <summary>
/// 可受傷介面
/// 玩家和敵人都實作這個介面，讓 Hitbox 不需要知道打到的是誰
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage);
}