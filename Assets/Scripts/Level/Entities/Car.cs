using System;
using System.Collections;
using System.Collections.Generic;
using Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    private ShortestPath path;
    private int currentWaypointIndex = 0;
    private bool isForward;
    [DisplayWithoutEdit] public Transform currentPoint;
    [DisplayWithoutEdit] public Zone destination;
    [DisplayWithoutEdit] public Exorcist exorcist;
    [DisplayWithoutEdit] public bool patrolling, fighting;
    [SerializeField] private SerializableDictionary<Exorcist, GameObject> meshes;
    public Action<Car> onArrivedEvent;

    public void SetDestination(ShortestPath path, bool isForward)
    {
        this.path = path;
        this.isForward = isForward;
        float shortestDistance = Mathf.Infinity, p;
        for (int i = 0; i < path.Length; i++)
            if ((p = Vector3.Distance(transform.position, path[i].position)) < shortestDistance)
            {
                shortestDistance = p;
                currentWaypointIndex = i;
                currentPoint = path[i];
            }
    }

    public void SetExorcist(Exorcist exorcist)
    {
        this.exorcist = exorcist;
        foreach (var mesh in meshes)
        {
            mesh.Value.SetActive(mesh.Key == exorcist);
        }
    }

    public void Move()
    {
        if (!gameObject.activeSelf) return;
        if (fighting && destination.State == Zone.ZoneState.Resting)
        {
            fighting = false;
            onArrivedEvent.Invoke(this);
        }
        if (path == null) return;
        if ((isForward && currentWaypointIndex >= path.Length) || (!isForward && currentWaypointIndex < 0))
        {
            OnArrived();
            return;
        }
        transform.forward = currentPoint.position - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, currentPoint.position, exorcist.CarSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentPoint.position) < 0.01f)
        {
            if (isForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= path.path.Length) OnArrived();
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0) OnArrived();
            }
            if (path != null)
                currentPoint = path.path[currentWaypointIndex];
        }
    }

    private void OnArrived()
    {
        path = null;
        if (destination != null && destination.State == Zone.ZoneState.ActingEvent)
        {
            destination.OnPoliceArrived(exorcist);
            fighting = true;
        }
        else
            onArrivedEvent.Invoke(this);
    }
}
