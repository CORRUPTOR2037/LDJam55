using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceSheet : ScriptableObject
{
    public int dayLength;
    public Vector2 timeToTakeEvent;
    public Vector2 eventSpawnTime;
    public int timeForImpFighting;
    public int totalDays;
    public float startRage, maxRage, rageDecreasePerSecond, suspicionRaisePerSecond, suspicionFadePerSecond;
    public int penaltyForDeath;
    public DaySetting[] daysSettings;
    public ExorcistsConfig[] exorcistsSettings;
    public DaySetting CurrentDay(int index) => daysSettings[Mathf.Clamp(index - 1, 0, daysSettings.Length - 1)];
    public ExorcistsConfig CurrentExorcistsSetting(float currentRage)
    {
        for (int i = 1; i < exorcistsSettings.Length; i++)
        {
            if (exorcistsSettings[i].maxRageLevel > currentRage)
                return exorcistsSettings[i - 1];
        }
        return exorcistsSettings[exorcistsSettings.Length - 1];
    }
}

[System.Serializable]
public class DaySetting
{
    public Demon[] availableDemons;
    public List<string> availableZones;
    public int availableImps;
}

[System.Serializable]
public class ExorcistsConfig
{
    public int maxRageLevel;
    public Exorcist[] exorcists;
    public bool patrolling = false;
}
