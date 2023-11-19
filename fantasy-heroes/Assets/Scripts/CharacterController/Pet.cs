using UnityEngine;

public class Pet : MonoBehaviour
{
    public Character Hero;

    private void Update()
    {
        if (Hero.targetEnemy != null)
        {

        }
    }

    public void PossitionSetting(Vector3 heroPos)
    {
        transform.position = new Vector3(heroPos.x + 1f, heroPos.y + 2, heroPos.z);
    }

}
