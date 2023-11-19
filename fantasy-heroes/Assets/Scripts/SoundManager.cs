using UnityEngine;

namespace FantasyHero
{
    public class SoundManager : MonoBehaviour
    {
        public AudioClip releasingBow, arrowFlying, footStep;
        public AudioClip LevelTransition;
        public AudioSource audioSource;

        public void PlayOnShotSound(string clip)
        {
            switch (clip)
            {
                case "ReleasingBow":
                    GameManager.Instance.PlayUISound(releasingBow);
                    break;
                case "ArrowFlying":
                    GameManager.Instance.PlayUISound(arrowFlying);
                    break;
                case "FootStep":
                    GameManager.Instance.PlayUISound(footStep);
                    break;
                case "LevelTransition":
                    GameManager.Instance.PlayUISound(LevelTransition);
                    break;
            }
        }
    }
}
