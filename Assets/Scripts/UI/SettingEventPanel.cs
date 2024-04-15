using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingEventPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eventName, eventDescription, eventType, impsCount;
    [SerializeField] private DemonCard demonCard;
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
        demonCard.gameObject.SetActive(false);
        eventName.text = zone.CurrentEvent.NameKey;
        eventDescription.text = zone.CurrentEvent.DescriptionKey;
        eventType.text = zone.CurrentEvent.ShortType;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        if (camera == null)
            camera = GameInstancesHolder.Get<LevelCamera>();
        camera.SetBlockerActive(true);
    }

    public void Close(bool result)
    {
        zone.SetSelected(false);
        gameObject.SetActive(false);
        camera.SetBlockerActive(false);
        Time.timeScale = 1;
        onClosed.Invoke(result);
    }

    public void AssignDemon(Demon demon)
    {
        this.demon = demon;
        imps = 0;
        impsCount.text = "0/" + maxImps;
        demonCard.gameObject.SetActive(true);
        demonCard.Setup(demon);
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
