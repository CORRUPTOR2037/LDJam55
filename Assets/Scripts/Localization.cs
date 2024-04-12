using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Localization : MonoBehaviour
{
    private static List<Localization> _allComponents = new List<Localization>();
    public static LocalizationTexts Texts;
    public static string CurrentLocKey { get; private set; } = "ru";
    public TMP_Text text;
    public string localizationKey;

    private void Awake()
    {
        _allComponents.Add(this);
        UpdateLoc();
    }

    private void UpdateLoc()
    {
        if (Texts == null)
            SetLocalization(CurrentLocKey);
        text.text = Texts.Get(localizationKey);
    }

    private void OnDestroy()
    {
        _allComponents.Remove(this);
    }

    public static void SetLocalization(string key)
    {
        CurrentLocKey = key;
        Texts = Resources.Load<LocalizationTexts>("Localization/" + key);
        foreach (var c in _allComponents)
            c.UpdateLoc();
    }
}
