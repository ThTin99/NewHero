using TMPro;
using UnityEngine;

/// <summary>
/// License: You are free to modify this file for your personal or commercial 
/// use. You may not sell or re-sell these scripts, or their derivatives, in 
/// any form other than their implementation as a system into your Unity project 
/// game/application.
/// </summary>

public class LanguageManager : MonoBehaviour
{
    public TMP_FontAsset fontKo;    // centralized font asset for KO lang
    public TMP_FontAsset fontRu;    // for RU - cyrillic langs
    public TMP_FontAsset fontEn;    // for all other latin/germanic langs
    public TMP_FontAsset fontCh;    // for China
    public TMP_FontAsset fontGe;    // for German
    public TMP_FontAsset fontIn;    // for Indo
    public TMP_FontAsset fontVi;    // for Vietnam
    public TMP_FontAsset fontJa;    // for Jap

    // simple singleton declaration
    private static LanguageManager _instance;
    public static LanguageManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<LanguageManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        SetLanguage(Lean.Localization.LeanLocalization.GetFirstCurrentLanguage());
    }

    // language change event definition
    public delegate void LanguageMgrHandler();
    public static event LanguageMgrHandler LanguageChanged;

    // call this method to properly fire the lang changed event
    private static void LanguageChangeHasOccurred()
    {
        if (LanguageChanged != null) LanguageChanged();
    }

    // specific to the M2H Localization Package -- pass standard
    // language codes.
    public void SetLanguage(string lang)
    {
        Lean.Localization.LeanLocalization.SetCurrentLanguageAll(lang);

        // inform systems and components, the language has been changed.
        LanguageChangeHasOccurred();
    }
}
