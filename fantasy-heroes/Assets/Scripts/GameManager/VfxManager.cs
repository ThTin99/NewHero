using System.Collections;
using UnityEngine;

public class VfxManager : Singleton<VfxManager>
{
    private GameObject currentVfx;
    [SerializeField] GameObject resurectionVfxGO, moveSmokeVfxGO, getDamageVfxGO, healVfxGO, destroyVfxGO;
    Transform holder;
    public TimeManager timeManager;
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        holder = GameManager.Instance.effectHolder;
    }

    public void PlayVFX(VFX_TYPE vfxType, Vector3 pos)
    {
        GameObject Vfx = null; 
        switch(vfxType) 
        {
            case VFX_TYPE.RESURECTION:
                Vfx = resurectionVfxGO;
                break;
            case VFX_TYPE.MOVE:
                Vfx = moveSmokeVfxGO;
                GameManager.Instance.soundManager.PlayOnShotSound("FootStep");
                break;
            case VFX_TYPE.GET_DAMAGE:
                Vfx = getDamageVfxGO;
                break;
            case VFX_TYPE.HEAL:
                Vfx = healVfxGO;
                break;
            case VFX_TYPE.DESTROY:
                Vfx = destroyVfxGO;
                break;
        }
        SpawnVfx(Vfx, pos);
    }

    public void SpawnVfx(GameObject _vfx, Vector3 _pos)
    {
        currentVfx = Instantiate(_vfx);
        currentVfx.transform.position = _pos + new Vector3(0,1f,0);
        currentVfx.transform.SetParent(holder);
        Vector3 temp;
        temp.x = 0;
        temp.y = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        currentVfx.transform.rotation = rotation;
    }

    public void PlayHealVfxAt(Vector3 position)
    {
        SpawnVfx(resurectionVfxGO, position);
    }
}

public enum VFX_TYPE
{
    RESURECTION, MOVE, GET_DAMAGE, HEAL, DESTROY
}
