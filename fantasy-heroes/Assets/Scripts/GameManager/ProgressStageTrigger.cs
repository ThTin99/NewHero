using UnityEngine;

public class ProgressStageTrigger : Singleton<ProgressStageTrigger>
{
    [SerializeField] Animator Animator;
    [SerializeField] ParticleSystem doorEffect;
    [SerializeField] GameObject normalGateObj, openedGateObj;
	bool isOpenDoor = false;
	
    private void Start()
    {
        CloseDoor();
    }

    public void OpenDoor()
    {
		if(isOpenDoor) return;
        if (Animator != null) Animator.SetBool("Open", true);
        if (doorEffect != null)
        {
            doorEffect.gameObject.SetActive(true);
            doorEffect.Play();
        }
        if (normalGateObj != null || openedGateObj != null) OpenGate(true);
		isOpenDoor = true;
    }

    public void CloseDoor()
    {
		if(!isOpenDoor) return;
        if (Animator != null) Animator.SetBool("Open", false);
        if (doorEffect != null)
        {
            doorEffect.gameObject.SetActive(false);
            doorEffect.Stop();
        }
        if (normalGateObj != null || openedGateObj != null) OpenGate(false);
		isOpenDoor = false;
    }

    void OpenGate(bool isOpen)
    {
        normalGateObj.SetActive(!isOpen);
        openedGateObj.SetActive(isOpen);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 5.87  -7.06   -3.87
            EventManager.TriggerEvent(EventManager.GameEventEnum.ON_TRANSITION_TO_NEXT_STAGE);
            GameManager.Instance.CleanObstacle();
            GameManager.Instance.CleanGameObject();
            GameManager.Instance.NextStage();
            GameManager.Instance.soundManager.PlayOnShotSound("LevelTransition");
            CloseDoor();
        }
        // else if (other.CompareTag("Player") && GameFlowManager.Instance.UserProfile.IsClearedTutorials == false)
        // {
        //     EventManager.TriggerEvent(EventManager.GameEventEnum.ON_TRANSITION_TO_NEXT_STAGE);
        //     CloseDoor();
        //     TutorialManager.Instance.NextStage();
        // }
    }
}