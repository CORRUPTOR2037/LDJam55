using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSystem : MonoBehaviour
{
    [SerializeField] private Road[] roads;
    [SerializeField] private Car carPrefab;
    [SerializeField] private List<Car> cars;

    List<ShortestPath> calculatedPaths = new List<ShortestPath>();

    private void Awake()
    {
        GameInstancesHolder.Register(this);
    }

    public Car SendNewCarTo(Transform start, Transform end)
    {
        Car car = null;
        for (int i = 0; i < cars.Count; i++)
        {
            if (!cars[i].gameObject.activeSelf)
            {
                car = cars[i];
                car.transform.position = start.position;
                break;
            }
        }
        if (car == null)
        {
            car = Object.Instantiate(carPrefab, transform);
            cars.Add(car);
            car.name = "Car " + cars.Count;
            car.currentPoint = start;
            car.transform.position = start.position;
        }

        SendCarTo(car, end);

        return car;
    }

    public void SendCarTo(Car car, Transform end)
    {
        ShortestPath path = null;
        bool isForward = false;
        foreach (var prevPath in calculatedPaths)
        {
            if (prevPath.IsForward(car.currentPoint, end))
            {
                path = prevPath;
                isForward = true;
                break;
            }
            else if (prevPath.IsBackward(car.currentPoint, end))
            {
                path = prevPath;
                isForward = false;
                break;
            }
        }
        if (path == null)
        {
            path = FindShortestPath(car.currentPoint, end);
            isForward = true;
        }
        car.SetDestination(path, isForward);
    }

    public ShortestPath FindShortestPath(Transform start, Transform end)
    {
        List<Transform> path = new List<Transform>();

        if (start == null || end == null)
        {
            Debug.LogWarning("Start or end transform is null.");
            path.Add(start);
            path.Add(end);
            return new ShortestPath(path);
        }

        // Initialize the A* algorithm
        Dictionary<Transform, float> distance = new Dictionary<Transform, float>();
        Dictionary<Transform, Transform> previous = new Dictionary<Transform, Transform>();
        List<Transform> unvisited = new List<Transform>();

        foreach (Road road in roads)
        {
            foreach (Transform point in road.points)
            {
                distance[point] = Mathf.Infinity;
                previous[point] = null;
                unvisited.Add(point);
            }
        }

        distance[start] = 0;
        float alt;
        Transform neighbor;

        // A* algorithm
        while (unvisited.Count > 0)
        {
            Transform current = null;
            foreach (Transform point in unvisited)
            {
                if (current == null || distance[point] < distance[current])
                {
                    current = point;
                }
            }

            unvisited.Remove(current);

            if (current == end) break;

            foreach (Road road in roads)
            {
                var index = System.Array.IndexOf(road.points, current);
                if (index < 0) continue;

                if (index - 1 >= 0)
                {
                    neighbor = road.points[index - 1];
                    alt = distance[current] + Vector3.Distance(current.position, neighbor.position);
                    if (alt < distance[neighbor])
                    {
                        distance[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
                if (index + 1 < road.points.Length)
                {
                    neighbor = road.points[index + 1];
                    alt = distance[current] + Vector3.Distance(current.position, neighbor.position);
                    if (alt < distance[neighbor])
                    {
                        distance[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }
        }

        // Reconstruct the shortest path
        Transform currentPoint = end;
        while (currentPoint != null)
        {
            path.Insert(0, currentPoint);
            currentPoint = previous[currentPoint];
        }
        var final = new ShortestPath(path);
        calculatedPaths.Add(final);
        return final;
    }

    private void Update()
    {
        foreach (Car car in cars)
            car.Move();
    }

    private Vector3 up = Vector3.up * 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (var road in roads)
        {
            if (road.points.Length > 0)
            {
                Gizmos.DrawIcon(road.points[0].position + up, "sv_icon_dot1_pix16_gizmo");
                Gizmos.DrawIcon(road.points[road.points.Length - 1].position + up, "sv_icon_dot1_pix16_gizmo");
            }
            for (int i = 0; i < road.points.Length - 1; i++)
            {
                Gizmos.DrawLine(road.points[i].position + up, road.points[i+1].position + up);
            }
        }
    }

    public void DeactivateAllCars()
    {
        foreach (Car car in cars)
        {
            car.destination = null;
            car.patrolling = false;
            car.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class Road 
{
    public Transform[] points;
}

[System.Serializable]
public class ShortestPath
{
    public ShortestPath(List<Transform> path)
    {
        this.path = path.ToArray();
    }
    public Transform[] path;
    public bool IsForward(Transform start, Transform end)
    {
        if (path[path.Length - 1] != end) return false;
        for (int i = 0; i < path.Length - 1; i++)
            if (path[i] == start) return true;
        return false;
    }

    public bool IsBackward(Transform start, Transform end) => IsForward(end, start);

    public Transform this[int i] => path[i];
    public int Length => path.Length;
}