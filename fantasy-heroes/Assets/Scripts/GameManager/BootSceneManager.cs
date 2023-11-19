using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootSceneManager : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TMPro.TMP_Text percentTxt;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        yield return null;
        yield return new WaitUntil(() => GameFlowManager.Instance != null);
        GameFlowManager.Instance.UserProfileSetting();
        var operation = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
            {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            percentTxt.text = $"{Mathf.FloorToInt(progress * 100)}%";
            yield return null;
        }
    }
}
