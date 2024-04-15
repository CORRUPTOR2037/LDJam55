using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceStation : MonoBehaviour
{
    [SerializeField] private Exorcist[] exorcistSlots;
    [SerializeField] private Transform startPoint;
    private RoadSystem roadSystem;
    private City city;

    public Car[] sentExorcists;

    private LinkedList<Zone> queue = new LinkedList<Zone>();
    private float clock = 0;
    private bool patrolling;

    private void Awake()
    {
        GameInstancesHolder.Register(this);
    }

    private void Start()
    {
        roadSystem = GameInstancesHolder.Get<RoadSystem>();
        city = GameInstancesHolder.Get<City>();
    }

    private void Update()
    {
        clock += Time.deltaTime;
        if (clock < 1) return;

        clock -= 1;
        if (!AnyCarAvailable()) return;

        Zone zone;
        if (queue.Count > 0)
        {
            zone = queue.First.Value;
            queue.RemoveFirst();
            Send(zone, false);
        }
        else if (patrolling)
        {
            zone = city.zones[Random.Range(0, city.zones.Length)];
            Send(zone, true);
        }
    }

    public void Reset(DaySetting settings)
    {
        queue.Clear();
        clock = 0;
        patrolling = settings.patrolling;
        exorcistSlots = settings.availableExorcists;
        roadSystem.DeactivateAllCars();
        sentExorcists = new Car[exorcistSlots.Length];
    }

    public void RequestTo(Zone zone)
    {
        queue.AddLast(zone);
    }

    private bool AnyCarAvailable()
    {
        for (int i = 0; i < sentExorcists.Length; i++)
            if (sentExorcists[i] == null || sentExorcists[i].destination == null)
                return true;
        return false;
    }

    private void Send(Zone zone, bool patrolling)
    {
        int slot = -1;
        for (int i = 0; i < sentExorcists.Length; i++)
            if (sentExorcists[i] == null || sentExorcists[i].destination == null)
            {
                slot = i;
                break;
            }
        
        if (slot < 0)
        {
            Debug.LogWarning("No one to sent on duty");
            return;
        }

        Car car;
        if (sentExorcists[slot] == null)
        {
            sentExorcists[slot] = car = roadSystem.SendNewCarTo(startPoint, zone.centerPoint);
        }
        else
        {
            car = sentExorcists[slot];
            roadSystem.SendCarTo(car, zone.centerPoint);
        }
        car.destination = zone;
        car.exorcist = exorcistSlots[slot];
        car.patrolling = patrolling;
        car.onArrivedEvent = OnCarArrivedToZone;
        car.gameObject.SetActive(true);
    }

    private void SendHome(Car car)
    {
        roadSystem.SendCarTo(car, startPoint);
        car.destination = null;
        car.onArrivedEvent = OnCarArrivedToHome;
    }

    private void OnCarArrivedToZone(Car car)
    {
        SendHome(car);
    }

    private void OnCarArrivedToHome(Car car)
    {
        for (int i = 0; i < sentExorcists.Length; i++)
            if (sentExorcists[i] == car)
            {
                sentExorcists[i] = null;
                car.gameObject.SetActive(false);
            }
    }
}
