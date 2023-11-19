using Lean.Localization;
using TMPro;
using UnityEngine;

/// <summary>
/// License: You are free to modify this file for your personal or commercial 
/// use. You may not sell or re-sell these scripts, or their derivatives, in 
/// any form other than their implementation as a system into your Unity project 
/// game/application.
/// 
/// Localization responder, configured to operate with the M2H Localization Package asset.
/// Assumes a singleton centralized manager class (LanguageManager{}) that maintains 
/// fields for the font assets for different language sets being parsed (in this case: KO, RU,
/// and EN -- all other). These centralized font assets may be overridden in the 
/// public fields here.
///
/// It is expected the LanguageManager defines an event used to indicate a language
/// change has occurred.
/// 
/// VERSION: 1.0.1
///     fixes: typo on delegate unsubscribe (-=) vice (+=)
/// </summary>

public class LocalizationResponder : MonoBehaviour
{
    private bool m_Initialized = false;
    private TMP_Text tmpText;       // text mesh pro text object.

    private void Awake()
    {
        // cache the TMP component on this object
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        OnLanguageChanged();
        m_Initialized = true;
    }

    private void OnEnable()
    {
        // subscribe to event for language change
        LanguageManager.LanguageChanged += OnLanguageChanged;
        LeanLocalization.OnLocalizationChanged += OnLanguageChanged;

        // Initialize the component on enable to make sure this object
        // has the most current language configuration.
        if (m_Initialized == true)
        {
            OnLanguageChanged();
        }
    }

    private void OnDisable()
    {
        LanguageManager.LanguageChanged -= OnLanguageChanged;
        LeanLocalization.OnLocalizationChanged -= OnLanguageChanged;
    }

    public void OnLanguageChanged()
    {
        // determine which language is being used:
        switch (LeanLocalization.GetFirstCurrentLanguage())
        {
            case "English":
                // apply the centralized font asset setting
                tmpText.font = LanguageManager.Instance.fontEn;
                break;
            case "Chinese":
                tmpText.font = LanguageManager.Instance.fontCh;
                break;
            case "Korean":
                tmpText.font = LanguageManager.Instance.fontKo;
                break;
            case "Russian":
                tmpText.font = LanguageManager.Instance.fontRu;
                break;
            case "German":
                tmpText.font = LanguageManager.Instance.fontGe;
                break;
            case "Indonesian":
                tmpText.font = LanguageManager.Instance.fontIn;
                break;
            case "Vietnamese":
                tmpText.font = LanguageManager.Instance.fontVi;
                break;
            case "Japanese":
                tmpText.font = LanguageManager.Instance.fontJa;
                break;
            default: // fall thru for all other languages; add additional cases if nec.
                tmpText.font = LanguageManager.Instance.fontEn;
                break;
        }
    }
}
