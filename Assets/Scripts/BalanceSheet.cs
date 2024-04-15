using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceSheet : ScriptableObject
{
    public int dayLength;
    public Vector2 timeToTakeEvent;
    public Vector2 eventSpawnTime;
    public int timeForImpFighting;
    public DaySetting[] daysSettings;
    public DaySetting CurrentDay(int index) => daysSettings[Mathf.Clamp(index - 1, 0, daysSettings.Length - 1)];
}

[System.Serializable]
public class DaySetting
{
    public Demon[] availableDemons;
    public Exorcist[] availableExorcists;
    public List<string> availableZones;
    public int rageTarget, availableImps;
    public bool patrolling = false;
}
