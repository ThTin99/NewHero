using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class swipe : MonoBehaviour
{
    public List<Transform> mapList = new List<Transform>();
    public Scrollbar scrollbar;
    public RectTransform backgroundToGetSize;
    public int halfSizeOfElement;
    private float scroll_pos = 0;
    float[] pos;
    private bool runIt = false;
    private float time;
    private Button takeTheBtn;
    int btnNumber;
    public int numberOfSceneChoosed = 1;

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private Lean.Gui.LeanWindow stageTransitionPopup;

    // Start is called before the first frame update
    void OnEnable()
    {
        MapSetting();
        int sizePaddingToCenter = (int)backgroundToGetSize.rect.width / 2;
        HorizontalLayoutGroup group = GetComponent<HorizontalLayoutGroup>();
        group.padding.left = sizePaddingToCenter - halfSizeOfElement;
        group.padding.right = sizePaddingToCenter - halfSizeOfElement;
    }

    void MapSetting()
    {
        for(int i=0; i< mapList.Count; i++)
        {
            if(GameFlowManager.Instance.UserProfile.chapterIndex < i || i > 2)
                BlockMap(mapList[i]);
            
            if(i > 2)
                MapCommingSoon(mapList[i]);
        }
    }

    void BlockMap(Transform map)
    {
        map.GetChild(3).gameObject.SetActive(true);
        map.GetChild(4).gameObject.SetActive(true);
    }

    void MapCommingSoon(Transform map)
    {
        map.GetChild(5).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        if (runIt)
        {
            GecisiDuzenle(distance, pos, takeTheBtn);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0;
                runIt = false;
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1.1f, 1.1f), 0.1f);
                numberOfSceneChoosed = i + 1;
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j != i)
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.9f, 0.9f), 0.1f);
                    }
                }
            }
        }
    }

    private void GecisiDuzenle(float distance, float[] pos, Button btn)
    {
        // btnSayi = System.Int32.Parse(btn.transform.name);

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                scrollbar.value = Mathf.Lerp(scrollbar.value, pos[btnNumber], 1f * Time.deltaTime);

            }
        }

        for (int i = 0; i < btn.transform.parent.transform.childCount; i++)
        {
            btn.transform.name = ".";
        }

    }
    public void WhichBtnClicked(Button btn)
    {
        btn.transform.name = "clicked";
        for (int i = 0; i < btn.transform.parent.transform.childCount; i++)
        {
            if (btn.transform.parent.transform.GetChild(i).transform.name == "clicked")
            {
                btnNumber = i;
                takeTheBtn = btn;
                time = 0;
                scroll_pos = (pos[btnNumber]);
                runIt = true;
            }
        }
    }

    public void GetScene()
    {
        if (!MapIsBlock())
        {
            if(EnergyManager.Instance.UseEnergy(-10))
            {
                Invoke(nameof(LoadMapWithSomeDelay), 0.5f);
                stageTransitionPopup.TurnOn();
                GameFlowManager.Instance.ChooseMap(numberOfSceneChoosed - 1);
            }
            else
            {
                NotificationManager.Instance.ShowNotifyWithContent("Not enough energy!");
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotifyWithContent("Map is blocking!");
        }
    }

    private bool MapIsBlock()
    {
        if(numberOfSceneChoosed >= 4) return true;
        return GameFlowManager.Instance.UserProfile.chapterIndex < numberOfSceneChoosed - 1;
    }

    private void LoadMapWithSomeDelay()
    {
        //load to map 1 and set index
        sceneLoader.LoadMapUsingAddessable(1);
    }
}