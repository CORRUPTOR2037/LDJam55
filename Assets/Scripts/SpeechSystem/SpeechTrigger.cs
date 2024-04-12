using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeechTrigger : MonoBehaviour
{
    public Speech speech;
    public bool OneTime = true;
    private bool activated = false;

    void OnTriggerEnter(Collider collider)
    {   
        if (speech == null) return;
        if (OneTime && activated) return;
        if (collider.gameObject.tag == "Player"){
            SpeechPlayer.Instance.Play(speech);
            activated = true;
        }
    }
}
