using UnityEngine;

public class RotateCharacterInspector : MonoBehaviour
{
    private Touch Touch;
    private Quaternion rotationY;
    [SerializeField] private float rotateSpeedModifier = 2f;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch = Input.GetTouch(0);

            if (Touch.phase == TouchPhase.Moved)
            {
                rotationY = Quaternion.Euler(0f, -Touch.deltaPosition.x * rotateSpeedModifier, 0f);

                transform.rotation = rotationY * transform.rotation;
            }
        }
    }
}
