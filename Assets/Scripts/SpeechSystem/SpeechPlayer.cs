using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechPlayer : MonoBehaviour
{
    public static SpeechPlayer Instance { get; private set; }

    public TextMeshProUGUI subtitlesText;
    public Button SkipButton;
    public float SecondsPerLetter;

    void Awake()
    {
        Instance = this;
        SkipButton.onClick.AddListener(TrySkip);
    }

    private string _targetText;
    private Speech _next;
    private float _timer;
    private bool _waitingForText;

    public void Play(Speech speech)
    {
        _next = speech.Next;
        _targetText = Localization.Texts.Get(speech.LocalizationKey);

        subtitlesText.text = "";
        gameObject.SetActive(true);

        if (!string.IsNullOrWhiteSpace(speech.Sound))
            AudioManager.PlayClip(speech.Sound);
        _timer = 0;
        _waitingForText = true;
    }

    private void Update()
    {
        if (_waitingForText)
        {
            _timer += Time.deltaTime;

            while (_timer > SecondsPerLetter)
            {
                if (_targetText.Length > subtitlesText.text.Length)
                {
                    subtitlesText.text = _targetText.Substring(0, subtitlesText.text.Length + 1);
                    _timer -= SecondsPerLetter;
                }
                else TrySkip();
            }
        }
        else
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
                TrySkip();
        }
    }

    private void TrySkip()
    {
        if (_waitingForText)
        {
            _waitingForText = false;
            _timer = 3;
        }
        else OnSpeechEnd();
    }

    private void OnSpeechEnd()
    {
        if (_next != null) Play(_next);
        else gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
