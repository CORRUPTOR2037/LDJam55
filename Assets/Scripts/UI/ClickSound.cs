using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour
{
    private static string clickSoundName = "click"; 
    void Start()
    {
        GetComponent<Button>()?.onClick.AddListener(() => AudioManager.PlayClip(clickSoundName));
        GetComponent<Toggle>()?.onValueChanged.AddListener((val) => AudioManager.PlayClip(clickSoundName));
    }
}
