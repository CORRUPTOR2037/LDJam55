using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Zone : MonoBehaviour
{
    [SerializeField] private Gradient suspicionGradient;
    [SerializeField] private SpriteRenderer selector;
    public Transform centerPoint;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ZoneTimer offeringTimer, actingTimer, fightingTimer;
    [SerializeField] private BalanceSheet balanceSheet;

    private Player player;
    private City city;
    private PoliceStation policeStation;

    public string zoneName;
    [DisplayWithoutEdit] public bool Selected = false;
    [DisplayWithoutEdit] public LevelEvent CurrentEvent = null;
    [DisplayWithoutEdit] public Demon AssignedDemon;
    [DisplayWithoutEdit] public Exorcist AssignedExorcist;
    [DisplayWithoutEdit] public int AssignedImps;
    [DisplayWithoutEdit] public float SuspicionLevel;
    public Action<Zone> onClick;

    private bool mouseOn = false;
    private float rageTimer, killImpTimer;

    public enum ZoneState
    {
        Resting, OfferingEvent, ActingEvent, Fighting
    }
    [DisplayWithoutEdit] public ZoneState State;

    public int DamagePerSecond => 
        AssignedDemon.rageMult + 
        AssignedImps + 
        (AssignedDemon.specialization == CurrentEvent.EventType ? 1 : 0);

    private void Start()
    {
        player = GameInstancesHolder.Get<Player>();
        city = GameInstancesHolder.Get<City>();
        policeStation = GameInstancesHolder.Get<PoliceStation>();
        canvas.worldCamera = GameInstancesHolder.Get<LevelCamera>().GetComponent<Camera>();
        Reset();
        canvas.transform.forward = GameInstancesHolder.Get<LevelCamera>().transform.forward;
        offeringTimer.onCompleted = () =>
        {
            CurrentEvent = null;
            player.OnEventDrop(this);
            State = ZoneState.Resting;
            OnStateUpdated();
        };
        fightingTimer.onCompleted = () =>
        {
            CurrentEvent = null;
            RevokeDemon();
            OnStateUpdated();
        };
    }

    public void Reset()
    {
        State = ZoneState.Resting;
        Selected = false;
        CurrentEvent = null;
        AssignedDemon = null;
        AssignedImps = 0;
        SuspicionLevel = 0;
        OnMouseExit();
        OnStateUpdated();
    }

    private void Update()
    {
        if (State == ZoneState.Fighting && AssignedImps > 0)
        {
            killImpTimer -= Time.deltaTime * AssignedExorcist.FightSpeedMult;
            if (killImpTimer < 0)
            {
                AssignedImps--;
                killImpTimer = balanceSheet.timeForImpFighting;
                fightingTimer.UpdateImpsCount(AssignedImps);
            }
        }
        if ((State == ZoneState.ActingEvent || State == ZoneState.Fighting) && SuspicionLevel < 0.99)
        {
            rageTimer += Time.deltaTime;
            if (rageTimer >= 1)
            {
                rageTimer -= 1;
                var damage = DamagePerSecond;
                SuspicionLevel = Mathf.Clamp01(SuspicionLevel + damage * balanceSheet.suspicionRaisePerSecond);
                city.AddRage(damage);
                UpdateColor();
            }
        }
        if (State == ZoneState.Resting || State == ZoneState.OfferingEvent)
        {
            SuspicionLevel = Mathf.Clamp01(SuspicionLevel - balanceSheet.suspicionFadePerSecond * Time.deltaTime);
            UpdateColor();
        }
    }

    private void OnStateUpdated()
    {
        switch (State)
        {
            case ZoneState.Resting:
                canvas.gameObject.SetActive(false);
                break;
            case ZoneState.OfferingEvent:
            {
                actingTimer.gameObject.SetActive(false);
                fightingTimer.gameObject.SetActive(false);
                if (!offeringTimer.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true);
                    offeringTimer.gameObject.SetActive(true);
                }
            } break;
            case ZoneState.ActingEvent:
            {
                offeringTimer.gameObject.SetActive(false);
                fightingTimer.gameObject.SetActive(false);
                if (!actingTimer.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true);
                    actingTimer.gameObject.SetActive(true);
                }
                rageTimer = 0;
                policeStation.RequestTo(this);
            }
            break;
            case ZoneState.Fighting:
            {
                offeringTimer.gameObject.SetActive(false);
                actingTimer.gameObject.SetActive(false);
                if (!fightingTimer.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true);
                    fightingTimer.gameObject.SetActive(true);
                }
            }
            break;
        }
    }
    
    void OnMouseOver()
    {
        if (!Input.GetMouseButton(0))
            UpdateColor();
        mouseOn = true;
    }

    void OnMouseExit()
    {
        UpdateColor();
        mouseOn = false;
    }

    void OnMouseDown() => SetSelected(!Selected);

    public void SetSelected(bool value)
    {
        Selected = value;
        UpdateColor();
        if (Selected)
            onClick.Invoke(this);
    }

    private Color CurrentColor(float alpha)
    {
        var color = suspicionGradient.Evaluate(SuspicionLevel);
        color.a = alpha;
        return color;
    }

    private void UpdateColor()
    {
        selector.color = CurrentColor(Selected ? 1 : Mathf.Max(SuspicionLevel, mouseOn ? 0.6f : 0.1f));
    }

    public void RevokeDemon()
    {
        if (AssignedDemon == null) return;

        if (State == ZoneState.Fighting && fightingTimer.timer < 0.1f)
        {
            player.KillDemon(AssignedDemon);
            city.AddRage(-balanceSheet.penaltyForDeath);
            AudioManager.PlayClip("demon_killed");
        }
        else
        {
            float damageToDemon = State == ZoneState.Fighting ? Mathf.Max(0, AssignedDemon.fightTime - fightingTimer.timer * AssignedExorcist.FightSpeedMult) : 0;
            player.RevokeDemon(AssignedDemon, AssignedImps, damageToDemon);
            AudioManager.PlayClip("return_demon");
        }
        State = ZoneState.Resting;
        AssignedDemon = null;
        OnStateUpdated();
    }

    public void AssignEvent(LevelEvent evt)
    {
        CurrentEvent = evt;
        State = ZoneState.OfferingEvent;
        OnStateUpdated();
        offeringTimer.SetupOffer(CurrentEvent, (int) UnityEngine.Random.Range(balanceSheet.timeToTakeEvent.x, balanceSheet.timeToTakeEvent.y));
    }

    public void StartAction()
    {
        State = ZoneState.ActingEvent;
        actingTimer.SetupAct(CurrentEvent, AssignedDemon, AssignedImps, DamagePerSecond);
        OnStateUpdated();
        AudioManager.PlayClip("demon_sent");
    }

    public void OnPoliceArrived(Exorcist exorcist)
    {
        State = ZoneState.Fighting;
        AssignedExorcist = exorcist;
        fightingTimer.SetupFight(AssignedDemon, (AssignedDemon.fightTime + AssignedImps * balanceSheet.timeForImpFighting) / exorcist.FightSpeedMult);
        fightingTimer.UpdateImpsCount(AssignedImps);
        killImpTimer = balanceSheet.timeForImpFighting;
        OnStateUpdated();
        AudioManager.PlayClip("demon_fight");
        AudioManager.PlayClip("battle");
    }
}