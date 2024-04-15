using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Demon[] demons;
    [SerializeField] private List<DemonCard> demonCards;
    [SerializeField] private DemonCard demonCardPrefab;
    [SerializeField] private TextMeshProUGUI impsCountText;
    [SerializeField] private SettingEventPanel settingEventPanel;

    private List<Demon> demonsOnDuty = new List<Demon>();
    private int currentImps, maxImps;
    private Zone selectedZone;

    private void Awake()
    {
        GameInstancesHolder.Register(this);
        settingEventPanel.onClosed = OnSettingPanelClosed;
    }

    public void Reset(DaySetting settings)
    {
        currentImps = maxImps = settings.availableImps;
        demons = settings.availableDemons;
        demonsOnDuty.Clear();
        selectedZone = null;
        while (demonCards.Count < demons.Length)
        {
            var card = Object.Instantiate(demonCardPrefab, demonCardPrefab.transform.parent);
            demonCards.Add(card);
        }
        foreach (var card in demonCards)
        {
            card.button.onClick.RemoveAllListeners();
            card.button.onClick.AddListener(() => OnDemonClicked(card.CardDemon));
            card.button.interactable = false;
            card.SetDeactivated(false);
            card.gameObject.SetActive(false);
        }
        for (int i = 0; i < demons.Length; i++)
        {
            demonCards[i].gameObject.SetActive(true);
            demonCards[i].Setup(demons[i]);
            demonCards[i].SetDeactivated(false);
        }
        impsCountText.text = currentImps.ToString();
    }

    public void RevokeDemon(Demon demon, int imps, float damageToDemon)
    {
        demonsOnDuty.Remove(demon);
        GetDemonCard(demon).SetTimeout(demon.reloadTime + damageToDemon);
        currentImps += imps;
        impsCountText.text = currentImps.ToString();
    }

    public void LaunchEventSetting(Zone zone)
    {
        foreach (var card in demonCards)
            if (!card.Deactivated)
                card.button.interactable = true;
        selectedZone = zone;
        settingEventPanel.availableImps = currentImps;
        settingEventPanel.Show(zone);
    }

    public void OnSettingPanelClosed(bool value)
    {
        foreach (var card in demonCards)
            card.button.interactable = false;
        if (value)
        {
            currentImps -= settingEventPanel.imps;
            selectedZone.AssignedDemon = settingEventPanel.demon;
            selectedZone.AssignedImps = settingEventPanel.imps;
            demonsOnDuty.Add(selectedZone.AssignedDemon);
            selectedZone.StartAction();
            GetDemonCard(selectedZone.AssignedDemon).SetDeactivated(true);
        }
    }

    public void OnEventDrop(Zone zone)
    {
        if (selectedZone == zone)
        {
            selectedZone = null;
            if (settingEventPanel.gameObject.activeSelf)
                settingEventPanel.Close(false);
        }
    }

    private void OnDemonClicked(Demon demon)
    {
        if (settingEventPanel.gameObject.activeSelf)
            settingEventPanel.AssignDemon(demon);
    }

    private DemonCard GetDemonCard(Demon demon)
    {
        foreach (var card in demonCards)
            if (card.CardDemon == demon) return card;
        return null;
    }
}
