using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject container;
    public Button startButton, returnButton;
    public GameObject info, options;
    public Toggle ruLanguage, enLanguage;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            startButton.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(true);
            SetEnabled(false);
        } 
        else
        {
            startButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(false);
            SetEnabled(true);
        }

        ruLanguage.onValueChanged.AddListener((value) => OnLanguageButtonClick( value ? "en" : "ru"));
        enLanguage.onValueChanged.AddListener((value) => OnLanguageButtonClick(!value ? "en" : "ru"));
        OnLanguageButtonClick(PlayerPrefs.GetString("language", "en"));
    }

    public void SetEnabled(bool value)
    {
        container.gameObject.SetActive(value);
    }

    public void OnLanguageButtonClick(string name)
    {
        Localization.SetLocalization(name);
        if (name == "ru"){
            ruLanguage.SetIsOnWithoutNotify(false);
            ruLanguage.interactable = false;
            enLanguage.SetIsOnWithoutNotify(true);
            enLanguage.interactable = true;
        } else if (name == "en"){
            ruLanguage.SetIsOnWithoutNotify(true);
            ruLanguage.interactable = true;
            enLanguage.SetIsOnWithoutNotify(false);
            enLanguage.interactable = false;
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
