public interface IDamageable
{
    void CalculateDamageAfterArmor(int damage, TypeOfDamage typeDamage);
    bool TryDodge();
}