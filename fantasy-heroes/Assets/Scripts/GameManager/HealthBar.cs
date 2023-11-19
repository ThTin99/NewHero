using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private TextMeshProUGUI healthValueTxt;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        matBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        AlignCamera();
        if (transform.eulerAngles.x != 90f)
        {
            transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }

    private void AlignCamera()
    {
        if (mainCamera != null)
        {
            var camXform = mainCamera.transform;
            var forward = transform.position - camXform.position;
            forward.Normalize();
            var up = Vector3.Cross(forward, camXform.right);
            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }

    [SerializeField] private MeshRenderer meshRenderer;
    private MaterialPropertyBlock matBlock;

    internal void UpdateParams(int currentHealth, int maxHealth)
    {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", currentHealth / (float)maxHealth);
        meshRenderer.SetPropertyBlock(matBlock);
        UpdateMaxHealth(currentHealth);
    }

    internal void UpdateMaxHealth(int healthValue)
    {
        healthValueTxt.text = healthValue.ToString();
    }
}
