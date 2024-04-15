using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemonCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image background, portrait, deactivatedOverlay;
    [SerializeField] private TextMeshProUGUI demonName, reloadStat, impsStat, tooltipText;
    [SerializeField] private Slider readySlider;
    [SerializeField] private Image tooltip;
    public Button button;

    public Demon CardDemon { get; private set; }

    public bool Deactivated => deactivatedOverlay.enabled;

    private float timer = -1;
    
    private void Update()
    {
        if (readySlider == null || timer < 0) return;
        timer -= Time.deltaTime;
        readySlider.value = timer;
        if (timer < 0)
            SetDeactivated(false);
    }

    public void Setup(Demon demon)
    {
        background.color = demon.Color;
        demonName.text = demon.NameKey;
        reloadStat.text = demon.reloadTime.ToString();
        impsStat.text = demon.impsCount.ToString();
        tooltipText.text = demon.DescriptionKey;
        portrait.sprite = demon.Sprite;
        CardDemon = demon;
        readySlider.gameObject.SetActive(false);
    }

    public void SetDeactivated(bool value)
    {
        deactivatedOverlay.enabled = value;
        readySlider.gameObject.SetActive(false);
        button.interactable = !value;
    }

    public void SetTimeout(float time)
    {
        readySlider.maxValue = time;
        timer = time;
        readySlider.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}
