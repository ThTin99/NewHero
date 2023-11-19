using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SettingPopupManager : MonoBehaviour
{
    private const string MUSICSETTINGKEY = "MusicSetting";
    private const string SOUNDSETTINGKEY = "SoundSetting";
    private const string QUALITYSETTINGKEY = "QualitySetting";
    [SerializeField] private TMP_Text playerIDTxt;
    [Header("Music")]
    [SerializeField] private Image m_MusicIcon;
    [SerializeField] private Sprite m_MusicOn;
    [SerializeField] private Sprite m_MusicOff;
    [Header("Sound")]
    [SerializeField] private Image m_SoundIcon;
    [SerializeField] private Sprite m_SoundOn;
    [SerializeField] private Sprite m_SoundOff;

    private bool m_PlayMusic = true;
    private bool m_PlaySound = true;

    void Start() {
        SetupMusic();
        SetupSound();
        SetupQuality();
        SettingPlayerID();
    }

    private void SettingPlayerID()
    {
        playerIDTxt.text = "Player Id: " + GameFlowManager.Instance.UserProfile.playerId;
    }

    private void SetupQuality()
    {
        if (PlayerPrefs.HasKey(QUALITYSETTINGKEY))
        {
            ChangeQualitySetting();
        }
        else
        {
            PlayerPrefs.SetInt(QUALITYSETTINGKEY, 5);
            PlayerPrefs.Save();
        }
    }

    private void SetupSound()
    {
        if (PlayerPrefs.HasKey(SOUNDSETTINGKEY))
        {
            var isToggleOn = PlayerPrefs.GetInt(SOUNDSETTINGKEY);
            m_PlaySound = isToggleOn == 1;
            ChangeSoundToggleIcon();
        }
        else
        {
            PlayerPrefs.SetInt(SOUNDSETTINGKEY, 1);
            PlayerPrefs.Save();
        }
    }

    private void SetupMusic()
    {
        if (PlayerPrefs.HasKey(MUSICSETTINGKEY))
        {
            var isToggleOn = PlayerPrefs.GetInt(MUSICSETTINGKEY);
            m_PlayMusic = isToggleOn == 1;
            ChangeMusicToggleIcon();
        }
        else
        {
            PlayerPrefs.SetInt(MUSICSETTINGKEY, 1);
            PlayerPrefs.Save();
        }

        SoundManager.SetVolumeMusic(0.3f);
    }

    public void ChangeQualitySetting()
    {
        var qualityLevel = PlayerPrefs.GetInt(QUALITYSETTINGKEY);
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    private void ChangeMusicToggleIcon()
    {
        SoundManager.SetDisableBGM(!m_PlayMusic);
        if (!m_PlayMusic)
        {
            SoundManager.StopMusic();
        }
        else
        {
            //SoundManager.PlayConnection(SoundManager.GetSoundConnectionForThisLevel(SceneManager.GetActiveScene().name));
        }
        m_MusicIcon.sprite = m_PlayMusic ? m_MusicOn : m_MusicOff;
    }

    private void ChangeSoundToggleIcon()
    {
        SoundManager.SetDisableSFX(!m_PlaySound);
        m_SoundIcon.sprite = m_PlaySound ? m_SoundOn : m_SoundOff;
    }

    public void ChangeMusicIcon()
    {
        m_PlayMusic = !m_PlayMusic;
        ChangeMusicToggleIcon();
        SoundManager.SetVolumeMusic(0.3f);
        PlayerPrefs.SetInt(MUSICSETTINGKEY, m_PlayMusic ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeSoundIcon()
    {
        m_PlaySound = !m_PlaySound;
        ChangeSoundToggleIcon();
        PlayerPrefs.SetInt(SOUNDSETTINGKEY, m_PlaySound ? 1 : 0);
        PlayerPrefs.Save();
    }
}
