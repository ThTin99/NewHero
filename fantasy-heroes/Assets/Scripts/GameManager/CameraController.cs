using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private const float CAMERA_TO_HERO_OFFSET = 25f;

    ////Variables
    private Transform player;
    private Vector3 velocity = Vector3.zero;
    private bool isReady = false;

    [SerializeField] private float smooth = 0.1f;

    internal void CameraSetup()
    {
        player = FindObjectOfType<Character>().transform;
        isReady = true;
    }

    //method
    private void FixedUpdate()
    {
        SnapCamera();
    }

    public void SnapCamera(bool immediately = false)
    {
        if (isReady)
        {
            Vector3 pos = new Vector3
            {
                x = player.position.x
            };
            pos.x = 0.5f;
            pos.y = player.position.y + CAMERA_TO_HERO_OFFSET;
            pos.z = player.position.z - 13;
            if (pos.z >= 0) pos.z = 0;
            if (pos.z <= -20f) pos.z = -20f;
            transform.position = immediately ? pos : Vector3.SmoothDamp(transform.position, pos, ref velocity, smooth);
        }
    }
}
