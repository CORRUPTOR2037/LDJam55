using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Event")]
public class LevelEvent : ScriptableObject
{
    public string NameKey, DescriptionKey;
    public ZoneEventType EventType;
    public int DifficultyLevel;

    public string ShortType => EventType.ToString()[0] + DifficultyLevel.ToString();

    public static Color EventColor(ZoneEventType type)
    {
        switch (type)
        {
            case ZoneEventType.Blue: return Color.blue;
            case ZoneEventType.Red: return Color.red;
            case ZoneEventType.Green: return Color.green;
            case ZoneEventType.Yellow: return Color.yellow;
        };
        return Color.white;
    }

    public Color Color => EventColor(EventType);
}
