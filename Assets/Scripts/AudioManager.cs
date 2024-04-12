using System.Collections;
using System.Collections.Generic;
using Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static float AudioVolume { get; private set; } = 1;
    public static float MusicVolume { get; private set; } = 1;

    private static AudioManager _current;

    [SerializeField] private AudioSource _music, _audio;

    [SerializeField] private SerializableDictionary<string, AudioClip> _clips;

    public static void SetAudioVolume(float value)
    {
        AudioVolume = value;
        _current._audio.volume = value;
    }

    public static void SetMusicVolume(float value)
    {
        MusicVolume = value;
        _current._music.volume = value;
    }

    private void Awake()
    {
        _music.volume = MusicVolume;
        _audio.volume = AudioVolume;

        _current = this;
    }

    public static void PlayClip(string name)
    {
        _current._audio.PlayOneShot(_current._clips[name]);
    }

    public static void PlayMusic()
    {
        if (!_current._music.isPlaying)
            _current._music.Play();
    }

    public static void StopMusic()
    {
        _current._music.Stop();
    }
}
