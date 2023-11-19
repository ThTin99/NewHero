using UnityEngine;

public class WorkshopTurret : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IRepaireable player))
        {
            player.Repairing();
            Destroy(gameObject);
        }
    }
}
