using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public Zone[] zones;
    public LevelEvent[] events;
    [SerializeField] private Vector2 hoursLimits;
    [SerializeField] private TextMeshProUGUI dayField, timeField;
    [SerializeField] private Slider dayProgressSlider, globalRageSlider;
    [SerializeField] private BalanceSheet balanceSheet;
    [SerializeField] private GameObject winPanel, losePanel, gameCompletedPanel;

    private int day = 0, hour, minute;
    private float dayProgress = 0, untilNextEvent;
    [DisplayWithoutEdit] public int targetRage, currentRage;

    private Player player;
    private PoliceStation policeStation;
    private new LevelCamera camera;

    private void Awake()
    {
        GameInstancesHolder.Register(this);
        foreach (var zone in zones)
            zone.onClick = OnZoneClick;
    }

    private void Start()
    {
        player = GameInstancesHolder.Get<Player>();
        camera = GameInstancesHolder.Get<LevelCamera>();
        policeStation = GameInstancesHolder.Get<PoliceStation>();
        Reset();
    }

    public void StartNewGame()
    {
        day = 0;
        Reset();
    }

    public void Reset()
    {
        day++;
        var daySettings = balanceSheet.CurrentDay(day);
        dayProgress = 0;
        untilNextEvent = 3;
        currentRage = 0;
        globalRageSlider.maxValue = targetRage = daySettings.rageTarget;
        globalRageSlider.value = 0;
        Time.timeScale = 1;
        foreach (var zone in zones)
        {
            zone.gameObject.SetActive(daySettings.availableZones.Contains(zone.zoneName));
            zone.Reset();
        }

        player.Reset(daySettings);
        policeStation.Reset(daySettings);
        camera.SetBlockerActive(false);
    }

    private void Update()
    {
        dayProgress += Time.deltaTime;
        if (dayProgress > balanceSheet.dayLength)
        {
            OnDayEnd(false);
            return;
        }

        // Calculate current hour and minute
        float ratio = Mathf.Clamp01(dayProgress / balanceSheet.dayLength); // Ensure ratio is between 0 and 1
        float totalHours = Mathf.Lerp(hoursLimits.x, hoursLimits.y, ratio); // Map ratio to the hours limits
        hour = Mathf.FloorToInt(totalHours);
        minute = Mathf.FloorToInt((totalHours - hour) * 60);

        dayField.text = "Day " + day;
        timeField.text = hour.ToString("00") + ":" + minute.ToString("00");
        dayProgressSlider.value = ratio;

        untilNextEvent -= Time.deltaTime;
        if (untilNextEvent < 0)
        {
            AssignEvent();
            untilNextEvent = Random.Range(balanceSheet.eventSpawnTime.x, balanceSheet.eventSpawnTime.y);
        }
    }

    private void AssignEvent()
    {
        List<Zone> possibleZones = new List<Zone>();
        foreach (var zone in zones)
            if (zone.gameObject.activeSelf && zone.State == Zone.ZoneState.Resting)
                possibleZones.Add(zone);
        
        if (possibleZones.Count == 0) return;
        var nextZone = possibleZones[Random.Range(0, possibleZones.Count)];
        var nextEvent = events[Random.Range(0, events.Length)];
        nextZone.AssignEvent(nextEvent);
    }

    public void OnZoneClick(Zone zone)
    {
        foreach (var other in zones)
            if (other != zone && other.Selected) other.SetSelected(false);
        if (zone.State == Zone.ZoneState.OfferingEvent)
            player.LaunchEventSetting(zone);
            
    }

    public void AddRage(int value)
    {
        currentRage = Mathf.Clamp(currentRage + value, 0, targetRage);
        globalRageSlider.value = currentRage;
        if (currentRage == targetRage)
            OnDayEnd(true);
    }

    private void OnDayEnd(bool result)
    {
        Time.timeScale = 0;
        camera.SetBlockerActive(true);

        if (result && day < balanceSheet.totalDays)
            gameCompletedPanel.SetActive(true);
        else if (result)
            winPanel.SetActive(true);
        else
            losePanel.SetActive(true);
    }
}
