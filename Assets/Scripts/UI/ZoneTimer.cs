using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eventTypeText, timerText, ragePerSecond, impsCount;
    [SerializeField] private Image border, demonImage;
    private bool isForward;

    public float timer { get; private set; }
    public Action onCompleted;

    public void SetupOffer(LevelEvent evt, int time)
    {
        border.color = evt.Color;
        eventTypeText.text = "Событие класса " + evt.ShortType;
        timer = time;
        isForward = false;
    }

    public void SetupAct(LevelEvent evt, Demon demon, int imps, int damagePerSecond)
    {
        border.color = evt.Color;
        eventTypeText.text = "Событие класса " + evt.ShortType;
        demonImage.sprite = demon.Sprite;
        ragePerSecond.text = damagePerSecond + "/sec";
        impsCount.text = imps.ToString();
        isForward = true;
        timer = 0;
    }

    public void SetupFight(Demon demon, float time)
    {
        eventTypeText.text = "Экзорцисты приехали!";
        demonImage.sprite = demon.Sprite;
        isForward = false;
        timer = time;
    }

    public void UpdateImpsCount(int count) => impsCount.text = count.ToString();

    private void Update()
    {
        if (isForward)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                gameObject.SetActive(false);
                onCompleted?.Invoke();
            }
        }
        timerText.text = ((int) timer / 60).ToString("00") + ":" + ((int) timer % 60).ToString("00");
    }
}
