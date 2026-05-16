using UnityEngine;
using TMPro;

public class AchievementTooltip : MonoBehaviour
{
    public static AchievementTooltip instance;
    public TMP_Text tooltipText;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}