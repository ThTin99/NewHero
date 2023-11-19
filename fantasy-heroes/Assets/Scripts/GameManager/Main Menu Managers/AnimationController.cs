using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animation Animation;
    [SerializeField] private string animationName;

    public void PlayIntroAnim()
    {
        Animation[animationName].normalizedTime = 0f;
        Animation[animationName].speed = 2;
        Animation.Play();
    }

    public void PlayOuttroAnim()
    {
        Animation[animationName].normalizedTime = 1f;
        Animation[animationName].speed = -2;
        Animation.Play();
    }
}
