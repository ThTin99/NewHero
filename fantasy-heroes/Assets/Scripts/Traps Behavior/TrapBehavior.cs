using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    [SerializeField] private int dmg;
    [SerializeField] private ushort destroyTime = 10;
    public bool enable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(dmg);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(dmg);
        }
    }

    private void Awake()
    {
        //Prevent trap projectile being stuck on the field
        if (enable)
        {
            Destroy(gameObject, destroyTime);
        }
    }
}
