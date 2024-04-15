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
    [DisplayWithoutEdit] public int AssignedImps;
    [DisplayWithoutEdit] public float SuspicionLevel;
    public Action<Zone> onClick;

    private bool mouseOn = false;
    private float rageTimer;

    public enum ZoneState
    {
        Resting, OfferingEvent, ActingEvent, Fighting
    }
    [DisplayWithoutEdit] public ZoneState State;

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
            State = ZoneState.Resting;
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
        if (State == ZoneState.ActingEvent)
        {
            rageTimer += Time.deltaTime;
            if (rageTimer >= 1)
            {
                rageTimer -= 1;
                city.AddRage(1);
            }
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
            selector.color = CurrentColor(Selected ? 1 : 0.6f);
        mouseOn = true;
    }

    void OnMouseExit()
    {
        selector.color = CurrentColor(Selected ? 1 : 0.3f);
        mouseOn = false;
    }

    void OnMouseDown() => SetSelected(!Selected);

    public void SetSelected(bool value)
    {
        Selected = value;
        selector.color = CurrentColor(Selected ? 1 : (mouseOn ? 0.6f : 0.3f));
        if (Selected)
            onClick.Invoke(this);
    }

    private Color CurrentColor(float alpha)
    {
        var color = suspicionGradient.Evaluate(SuspicionLevel);
        color.a = alpha;
        return color;
    }

    public void RevokeDemon()
    {
        State = ZoneState.Resting;
        player.RevokeDemon(AssignedDemon, AssignedImps);
        OnStateUpdated();
    }

    public void AssignEvent(LevelEvent evt)
    {
        CurrentEvent = evt;
        State = ZoneState.OfferingEvent;
        OnStateUpdated();
        offeringTimer.SetupOffer(CurrentEvent, 30);
    }

    public void StartAction()
    {
        State = ZoneState.ActingEvent;
        actingTimer.SetupAct(CurrentEvent, AssignedDemon, AssignedImps);
        OnStateUpdated();
    }

    public void OnPoliceArrived()
    {
        State = ZoneState.Fighting;
        fightingTimer.SetupFight(AssignedDemon, AssignedDemon.fightTime + AssignedImps * balanceSheet.timeForImpFighting);
        OnStateUpdated();
    }
}