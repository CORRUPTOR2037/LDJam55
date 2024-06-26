using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingEventPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eventName, eventDescription, eventType, impsCount, demonName;
    [SerializeField] private GameObject demonCard;
    [SerializeField] private Image demonImage, eventColor;
    [SerializeField] private Button plusImp, minusImp;

    [DisplayWithoutEdit] public int imps, availableImps;
    [DisplayWithoutEdit] public Demon demon;

    public Action<bool> onClosed;

    private new LevelCamera camera;

    private int maxImps => Mathf.Min(availableImps, demon.impsCount);
    private Zone zone;

    private void Start()
    {
        plusImp.onClick.AddListener(() => ChangeImps(1));
        minusImp.onClick.AddListener(() => ChangeImps(-1));
    }

    public void Show(Zone zone)
    {
        this.zone = zone;
        demonCard.SetActive(false);
        eventName.text = zone.CurrentEvent.NameKey;
        eventDescription.text = zone.CurrentEvent.DescriptionKey;
        eventType.text = zone.CurrentEvent.ShortType;
        eventColor.color = zone.CurrentEvent.Color;
        gameObject.SetActive(true);
        if (camera == null)
            camera = GameInstancesHolder.Get<LevelCamera>();
        camera.SetBlockerActive(true);
    }

    public void Close(bool result)
    {
        zone.SetSelected(false);
        gameObject.SetActive(false);
        camera.SetBlockerActive(false);
        onClosed.Invoke(result);
    }

    public void AssignDemon(Demon demon)
    {
        this.demon = demon;
        imps = 0;
        impsCount.text = "0/" + maxImps;
        demonCard.SetActive(true);
        demonName.text = demon.NameKey;
        demonImage.sprite = demon.Sprite;
        minusImp.gameObject.SetActive(false);
        plusImp.gameObject.SetActive(maxImps > 0);
    }

    private void ChangeImps(int dir)
    {
        int max = maxImps;
        imps = Mathf.Clamp(imps + dir, 0, max);
        impsCount.text = imps + "/" + max;
        plusImp.gameObject.SetActive(imps < max);
        minusImp.gameObject.SetActive(imps > 0);
    }
}
