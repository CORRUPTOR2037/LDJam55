using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent events;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
            events.Invoke();
    }

}
