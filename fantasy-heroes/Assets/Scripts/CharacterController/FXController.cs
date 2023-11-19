using System.Collections;
using UnityEngine;

public enum FXState { APPEAR, GETDAMAGE, DEAD, MOVE }
public class FXController : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => VfxManager.Instance != null);
    }

    Transform weapon, body;
    public void GetPos()
    {
        weapon = transform.GetComponent<Character>().weaponPivot;
        body = transform;
    }
    public void PlayFX(FXState _fXState)
    {
        Vector3 pos = new Vector3(body.position.x, 0.5f, body.position.z);
        switch (_fXState)
        {
            case FXState.APPEAR:
                VfxManager.Instance.PlayVFX(VFX_TYPE.RESURECTION ,pos);
                break;
            case FXState.GETDAMAGE:
                VfxManager.Instance.PlayVFX(VFX_TYPE.GET_DAMAGE, pos);
                break;
            case FXState.MOVE:
                VfxManager.Instance.PlayVFX(VFX_TYPE.MOVE, pos);
                break;
            case FXState.DEAD:
                VfxManager.Instance.PlayVFX(VFX_TYPE.DESTROY, pos);
                break;    
        }
    }
}
