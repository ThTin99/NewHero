using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RewardTower : MonoBehaviour
{
    [SerializeField] Transform openUp;
    private void Start()
    {
        StartCoroutine(Init());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICanRollLuckyWheel player))
        {
            player.ShowLuckyWheel();
            Destroy(gameObject);
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        openUp.DORotate(new Vector3(-140f, 180, 0), 0.2f, RotateMode.Fast);
        transform.DOShakePosition(0.25f, 0.3f, 20);
    }
}
