using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public float slowdownFactor = 0.05f;
    public float slowdownLength = 2f;

    internal bool IgnoreUpdate = false;//use when time is stop

    void Update()
    {
        if (IgnoreUpdate)
        {
            return;
        }

        if (Time.timeScale < 1)
        {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }
    }

    public void DoSlowmotion()
    {
        //Time.timeScale = slowdownFactor;
    }
}
