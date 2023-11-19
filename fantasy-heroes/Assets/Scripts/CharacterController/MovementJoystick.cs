using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    public GameObject joystick, joystickBG, border, borderUpRight, borderUpLeft, borderDownRight, borderDownLeft;
    public Vector2 joystickVec;
    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private float joystickRadius;
    public bool isFree;

    // Start is called before the first frame update
    void Start()
    {
        isFree = true;
        joystickOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 4;
    }


    public void PointerDown()
    {
        joystick.transform.position = Input.mousePosition;
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;
        border.transform.position = Input.mousePosition;
        BorderShow(true);
    }

    public void Drag(BaseEventData baseEventData)
    {
        isFree = false;
        PointerEventData pointEvenData = baseEventData as PointerEventData;
        Vector2 dragPos = pointEvenData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos);

        if (joystickDist < joystickRadius)
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }
        else
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }

        BorderShow(false);
        BorderManager(joystickVec);
    }

    public void PointerUp()
    {
        isFree = true;
        joystickVec = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
        joystickBG.transform.position = joystickOriginalPos;
        BorderShow(false);
    }

    public bool CheckFree()
    {
        return isFree;
    }

    void BorderShow(bool isShow)
    {
        borderUpRight.SetActive(isShow);
        borderUpLeft.SetActive(isShow);
        borderDownRight.SetActive(isShow);
        borderDownLeft.SetActive(isShow);
    }

    void BorderManager(Vector2 pos)
    {
        if (pos.x >= 0)
        {
            if (pos.y >= 0)
            {
                borderUpRight.SetActive(true);
            }
            else
            {
                borderDownRight.SetActive(true);
            }
        }
        else
        {
            if (pos.y >= 0)
            {
                borderUpLeft.SetActive(true);
            }
            else
            {
                borderDownLeft.SetActive(true);
            }
        }
    }
}
