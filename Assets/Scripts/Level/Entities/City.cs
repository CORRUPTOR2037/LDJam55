using System.Collections;
using System.Collections.Generic;
using Collections;
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
    [SerializeField] private TutorialWindow tutorialWindow;
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private SerializableDictionary<float, Toggle> timeToggles;

    private int day = 0, hour, minute;
    private float dayProgress = 0, untilNextEvent;
    [DisplayWithoutEdit] public float maxRage, currentRage;

    private Player player;
    private PoliceStation policeStation;
    private new LevelCamera camera;

    private void Awake()
    {
        GameInstancesHolder.Register(this);
        foreach (var zone in zones)
            zone.onClick = OnZoneClick;
        foreach (var toggle in timeToggles)
            toggle.Value.onValueChanged.AddListener((val) => SetMultiplier(toggle.Key));
    }

    private void Start()
    {
        player = GameInstancesHolder.Get<Player>();
        camera = GameInstancesHolder.Get<LevelCamera>();
        policeStation = GameInstancesHolder.Get<PoliceStation>();
        StartNewGame();

        if (!TutorialWindow.ShowedTutorial)
            tutorialWindow.Show();
    }

    public void StartNewGame()
    {
        day = 0;
        currentRage = balanceSheet.startRage;
        globalRageSlider.maxValue = maxRage = balanceSheet.maxRage;
        Reset();
    }

    public void Reset()
    {
        day++;
        var daySettings = balanceSheet.CurrentDay(day);
        var exorcistsSettings = balanceSheet.CurrentExorcistsSetting(currentRage);
        dayProgress = 0;
        untilNextEvent = 3;
        
        globalRageSlider.value = currentRage;
        Time.timeScale = 1;
        foreach (var zone in zones)
        {
            zone.gameObject.SetActive(daySettings.availableZones.Contains(zone.zoneName));
            zone.Reset();
        }

        player.Reset(daySettings);
        policeStation.Reset(exorcistsSettings);
        camera.SetBlockerActive(false);
        Update();
    }

    private void Update()
    {
        dayProgress += Time.deltaTime;
        if (dayProgress > balanceSheet.dayLength)
        {
            OnDayEnd(true);
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
        sun.color = sunColor.Evaluate(ratio);
        sun.intensity = sunColor.Evaluate(ratio).a * 1.3f;
        sun.transform.localEulerAngles = new Vector3(Mathf.Lerp(60, 180, ratio), -200, 0);
        currentRage -= Time.deltaTime * balanceSheet.rageDecreasePerSecond;
        globalRageSlider.value = currentRage;

        untilNextEvent -= Time.deltaTime;
        if (untilNextEvent < 0)
        {
            AssignEvent();
            untilNextEvent = Random.Range(balanceSheet.eventSpawnTime.x, balanceSheet.eventSpawnTime.y);
        }

        if (currentRage <= 0)
        {
            OnDayEnd(false);
        }
    }

    private void AssignEvent()
    {
        List<Zone> possibleZones = new List<Zone>();
        foreach (var zone in zones)
            if (zone.gameObject.activeSelf && zone.State == Zone.ZoneState.Resting && zone.SuspicionLevel < 0.9f)
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
        currentRage = Mathf.Clamp(currentRage + value, 0, maxRage);
    }

    private void OnDayEnd(bool result)
    {
        Time.timeScale = 0;
        camera.SetBlockerActive(true);

        if (result && day >= balanceSheet.totalDays)
            gameCompletedPanel.SetActive(true);
        else if (result)
            winPanel.SetActive(true);
        else
            losePanel.SetActive(true);
    }

    [DisplayWithoutEdit] public float SelectedTimeScale = 1;

    public void SetMultiplier(float val)
    {
        Time.timeScale = SelectedTimeScale = val;
        foreach (var toggle in timeToggles)
            toggle.Value.SetIsOnWithoutNotify(Mathf.Approximately(val, toggle.Key));
    }

    public void RevertTimeScale()
    {
        Time.timeScale = SelectedTimeScale;
    }
}
