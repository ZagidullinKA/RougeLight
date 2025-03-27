public interface IDamageable
{
    void TakeDamage(int damage, TypeOfDamage typeDamage);
    bool TryDodge();
}