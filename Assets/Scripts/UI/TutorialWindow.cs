using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : MonoBehaviour
{
    public static bool ShowedTutorial = false;

    [SerializeField] [TextArea(3, 10)] private string[] texts;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button left, right;

    private int selected;
    private new LevelCamera camera;

    private void Start()
    {
        ShowedTutorial = true;
        left.onClick.AddListener(() => ShowText(selected - 1));
        right.onClick.AddListener(() => ShowText(selected + 1));
    }
    
    public void Show()
    {
        Time.timeScale = 0;
        selected = 0;
        gameObject.SetActive(true);
        ShowText(0);
        camera = GameInstancesHolder.Get<LevelCamera>();
        camera.SetBlockerActive(true);
    }

    public void ShowText(int index)
    {
        selected = index;
        descriptionText.text = texts[index];
        left.gameObject.SetActive(index > 0);
        right.gameObject.SetActive(index < texts.Length - 1);
    }

    public void Hide()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        camera.SetBlockerActive(false);
    }
}
