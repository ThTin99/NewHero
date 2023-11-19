using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMapUsingAddessable(int num)
    {
        SceneManager.LoadScene(num + 1);

        SetTimeScaleToNormal();
    }

    private static void SetTimeScaleToNormal()
    {
        Time.timeScale = 1f;
    }
}