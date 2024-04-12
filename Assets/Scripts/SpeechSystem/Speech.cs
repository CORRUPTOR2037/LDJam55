using UnityEngine;

[CreateAssetMenu(menuName = "Speech Item")]
public class Speech : ScriptableObject
{
    public string LocalizationKey;
    public string Sound;
    public Speech Next;
}
