using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Character character;
    private Vector3 pointTargetMove;
    private MovementJoystick movementjoystick;
    //float timeToPower = 2f;

    [SerializeField] private float playerSpeed;

    // Start is called before the first frame update
    private void Awake()
    {
        character = GetComponent<Character>();
        movementjoystick = FindObjectOfType<MovementJoystick>();
    }

    private void FixedUpdate()
    {
        MoveByJoyStick();
    }

    void MoveByJoyStick()
    {
        pointTargetMove = new Vector3(movementjoystick.joystickVec.x, movementjoystick.joystickVec.y, 0f);
        if (!movementjoystick.CheckFree()) Move();
        else Idle();
    }

    void Move()
    {
        if (character.characterState == CharacterState.DEAD || character.attackController.isPowerShotting) return;
        character.rb.velocity = new Vector3(pointTargetMove.x * playerSpeed, 0.2f, pointTargetMove.y * playerSpeed);
        character.SetState(CharacterState.MOVE);
        character.SetAnimation("Run");
        character.MoveRotate(pointTargetMove);
        character.attackController.CheckingMove(true);
    }

    public void Idle()
    {
        if (character.characterState == CharacterState.ATTACK || character.characterState == CharacterState.DEAD) return;
        character.characterState = CharacterState.IDLE;
        character.rb.velocity = Vector3.zero;
        character.attackController.CheckingMove(false);
        character.SetAnimation("Idle");
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_JOYSTICK_IDLE);
    }
}
