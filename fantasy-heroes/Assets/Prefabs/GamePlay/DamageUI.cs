using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageUI : MonoBehaviour
{
    public static DamageUI Instance { get; private set; }

    public class ActiveText
    {
        public TextMeshProUGUI UIText;
        public float MaxTime;
        public float Timer;
        public Vector3 WorldPositionStart;
        public int randY, randX;

        public void PlaceText(Camera cam, Canvas canvas)
        {
            float ratio = (1 - (Timer / MaxTime));
            Vector3 pos = WorldPositionStart + new Vector3(ratio * randX, Mathf.Sin(ratio * Mathf.PI) * randY, 0);
            pos = cam.WorldToScreenPoint(pos);
            //pos *= canvas.scaleFactor;
            pos.z = 0.0f;

            UIText.transform.position = pos;
        }
    }

    public TextMeshProUGUI DamageTextPrefab;

    Canvas m_Canvas;
    Queue<TextMeshProUGUI> m_TextPool = new Queue<TextMeshProUGUI>();
    List<ActiveText> m_ActiveTexts = new List<ActiveText>();

    Camera m_MainCamera;

    private int[] _randListY = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    private int[] _randListX = new int[] { -2, -1, 0, 1, 2 };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_Canvas = GetComponent<Canvas>();

        const int POOL_SIZE = 200;
        for (int i = 0; i < POOL_SIZE; ++i)
        {
            var t = Instantiate(DamageTextPrefab, m_Canvas.transform);
            t.gameObject.SetActive(false);
            m_TextPool.Enqueue(t);
        }

        m_MainCamera = Camera.main;

        if (PlayerPrefs.HasKey("PopDmgSetting"))
        {
            canPopDmg = PlayerPrefs.GetInt("PopDmgSetting") == 1;
        }
    }

    void Update()
    {
        for (int i = 0; i < m_ActiveTexts.Count; ++i)
        {
            var at = m_ActiveTexts[i];
            at.Timer -= Time.deltaTime * 2.5f;

            if (at.Timer <= -1.0f)
            {
                at.UIText.gameObject.SetActive(false);
                m_TextPool.Enqueue(at.UIText);
                m_ActiveTexts.RemoveAt(i);
                i--;
            }
            else if (at.Timer > 0f)
            {
                var color = at.UIText.color;
                color.a = 1;
                at.UIText.color = color;
                at.PlaceText(m_MainCamera, m_Canvas);
            }
            else if (at.Timer < 0 && at.Timer >= -1)
            {
                var color = at.UIText.color;
                color.a = 1 - Mathf.Abs(at.Timer);
                at.UIText.color = color;
            }
        }
    }

    private bool canPopDmg = true;

    public void DisablePopDmg()
    {
        canPopDmg = false;
        PlayerPrefs.SetInt("PopDmgSetting", 0);
    }

    public void EnablePopDmg()
    {
        canPopDmg = true;
        PlayerPrefs.SetInt("PopDmgSetting", 1);
    }

    /// <summary>
    /// Called by the CharacterData system when a new damage is made. This will take care of grabbing a text from
    /// the pool and place it properly, then register it as an active text so its position and opacity is updated by
    /// the system.
    /// </summary>
    /// <param name="amount">The amount of damage to display</param>
    /// <param name="worldPos">The position is the world where the damage text should appear (e.g. character head)</param>
    public void NewDamage(int amount, Vector3 worldPos, bool isString = false, string dodgeTxt = "")
    {
        if (canPopDmg)
        {
            var t = m_TextPool.Dequeue();

            t.text = isString ? dodgeTxt : amount.ToString();
            t.gameObject.SetActive(true);

            int randY = _randListY[Random.Range(0, _randListY.Length)];
            int randX = _randListX[Random.Range(0, _randListX.Length)];

            ActiveText at = new ActiveText();
            at.randX = randX;
            at.randY = randY;
            at.MaxTime = 1.0f;
            at.Timer = at.MaxTime;
            at.UIText = t;

            at.WorldPositionStart = worldPos + new Vector3(0.75f, 0f, 1.75f) + Vector3.up;
            at.PlaceText(m_MainCamera, m_Canvas);
            m_ActiveTexts.Add(at);
        }
    }

    public class ExtendedActiveText : ActiveText
    {
        public void PlaceText(Camera cam, Vector3 pos)
        {
            UIText.rectTransform.position = pos;
        }
    }

    public void ShowAbilityDescription(string description, Vector3 pos)
    {
        var t = m_TextPool.Dequeue();

        t.text = description;
        t.gameObject.SetActive(true);

        ExtendedActiveText at = new ExtendedActiveText
        {
            MaxTime = 1.5f
        };
        at.Timer = at.MaxTime;
        at.UIText = t;
        at.UIText.alignment = TextAlignmentOptions.Center;

        at.PlaceText(m_MainCamera, pos);
        m_ActiveTexts.Add(at);
    }
}