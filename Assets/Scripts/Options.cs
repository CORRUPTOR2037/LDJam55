using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider audioVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    void Start()
    {
        audioVolumeSlider.SetValueWithoutNotify(AudioManager.AudioVolume);
        audioVolumeSlider.onValueChanged.AddListener((val) => AudioManager.SetAudioVolume(val));

        musicVolumeSlider.SetValueWithoutNotify(AudioManager.MusicVolume);
        musicVolumeSlider.onValueChanged.AddListener((val) => AudioManager.SetMusicVolume(val));
    }
}
