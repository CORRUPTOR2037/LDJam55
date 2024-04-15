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

    private static Color RedZone = new Color(0.83f, 0.1f, 0.05f);
    private static Color BlueZone = new Color(0.25f, 0.45f, 0.95f);
    private static Color GreenZone = new Color(0.53f, 0.9f, 0.15f);
    private static Color YellowZone = new Color(1f, 0.8f, 0.15f);

    public static Color EventColor(ZoneEventType type)
    {
        switch (type)
        {
            case ZoneEventType.Blue: return BlueZone;
            case ZoneEventType.Red: return RedZone;
            case ZoneEventType.Green: return GreenZone;
            case ZoneEventType.Yellow: return YellowZone;
        };
        return Color.white;
    }

    public Color Color => EventColor(EventType);
}
