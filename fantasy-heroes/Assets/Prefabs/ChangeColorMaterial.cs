using UnityEngine;
using DG.Tweening;

public class ChangeColorMaterial : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _mSkinnedMeshRenderer;
    float timeShowChanged = 0f;
    bool isNormal = true;
    Color colorOrigin;
    [SerializeField] private GameObject bloodExplosion;
    [SerializeField] private bool NotShake;

    private void Start()
    {   
        colorOrigin = _mSkinnedMeshRenderer[0].materials[0].GetColor("_Color");
    }

    void Shake(){
        transform.DOShakePosition(0.2f,0.7f,30);
    }

    public void ChangeColorToDamaged()
    {
        if(!isNormal) return;
        if(!NotShake) Shake();
        isNormal = false;
        timeShowChanged = 0.25f;
        for (int i = 0; i < _mSkinnedMeshRenderer.Length; i++)
        {
            _mSkinnedMeshRenderer[i].materials[0].DOColor(Color.red, "_Color", 0.2f);
        }

        Vector3 pos = new Vector3(transform.position.x,2f,transform.position.z);
        GameObject effect = Instantiate(bloodExplosion, pos, Quaternion.identity, GameManager.Instance.effectHolder);
    }

    public void ChangeColorToDie()
    {
        if(NotShake) return;
        isNormal = false;
        timeShowChanged = 0.25f;
        for (int i = 0; i < _mSkinnedMeshRenderer.Length; i++)
        {
            _mSkinnedMeshRenderer[i].materials[0].DOColor(Color.black, "_Color", 2f);
        }

    }


    public void ChangeColorToNormal()
    {
        if (isNormal) return;
        isNormal = true;
        for (int i = 0; i < _mSkinnedMeshRenderer.Length; i++)
        {
            _mSkinnedMeshRenderer[i].materials[0].DOColor(colorOrigin, "_Color", 0.2f);
        }
    }

    private void Update()
    {
        if (timeShowChanged > 0)
        {
            timeShowChanged -= Time.deltaTime;
        }

        if (!isNormal && timeShowChanged < 0)
        {
            ChangeColorToNormal();
        }
    }
}
