using System.Collections;
using System.Collections.Generic;
using Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Localization file")]
public class LocalizationTexts : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, string> texts = new SerializableDictionary<string, string>();

    public string Get(string key)
    {
        if (texts.ContainsKey(key)) return texts[key];
        Debug.LogWarning("No localization for key '" + key + "'");
        return "";
    }
}
