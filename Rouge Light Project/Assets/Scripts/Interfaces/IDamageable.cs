public interface IDamageable
{
    void TakeDamage(int? damage);
    void AddDot(DotEffect dot);
    void UpdateDots();
}