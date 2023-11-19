public interface IDamageable
{
    void Damage(int amount, bool dealthBlow = false, bool crit = false, bool dealMoreDamageBasedOnHp = false);
}
