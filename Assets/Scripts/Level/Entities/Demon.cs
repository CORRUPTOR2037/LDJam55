using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demon")]
public class Demon : ScriptableObject
{
    public string NameKey;
    public Sprite Sprite;
    public int reloadTime, fightTime, impsCount, rageMult;
    public ZoneEventType specialization;

    public Color Color => LevelEvent.EventColor(specialization);
}

public enum ZoneEventType
{
    Red, Green, Blue, Yellow
}