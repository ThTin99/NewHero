using UnityEngine;
using UnityEngine.UI;

public class SliderPanelController : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private Slider joystickSizeSlider;
    [SerializeField] private Slider joystickOpaquenessSlider;

    public void UpdateJoystickSize()
    {
        try
        {
            joystick.transform.localScale = new Vector2(joystickSizeSlider.value, joystickSizeSlider.value);
        }
        catch (System.Exception)
        { }
    }

    public void UpdateJoystickOpaqueness()
    {
        try
        {
            var image = joystick.transform.GetChild(0).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, joystickOpaquenessSlider.value);
            var image1 = joystick.transform.GetChild(1).GetComponent<Image>();
            image1.color = new Color(image1.color.r, image1.color.g, image1.color.b, joystickOpaquenessSlider.value);
        }
        catch (System.Exception)
        { }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("SliderOpaqueness"))
        {
            joystickOpaquenessSlider.value = PlayerPrefs.GetFloat("SliderOpaqueness");
            UpdateJoystickOpaqueness();
        }
        if (PlayerPrefs.HasKey("SliderSize"))
        {
            joystickSizeSlider.value = PlayerPrefs.GetFloat("SliderSize");
            UpdateJoystickSize();
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("SliderSize", joystickSizeSlider.value);
        PlayerPrefs.SetFloat("SliderOpaqueness", joystickOpaquenessSlider.value);

        PlayerPrefs.Save();
    }
}
