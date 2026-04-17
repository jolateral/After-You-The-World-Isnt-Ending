using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI nudgeCountText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI careNotificationText;

    [Header("Buttons")]
    public Button skipDayButton;
    public Button endDayButton;

    GameData gameData;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        gameData = DayManager.Instance.gameData;
        skipDayButton?.onClick.AddListener(DayManager.Instance.SkipDay);
        endDayButton?.onClick.AddListener(DayManager.Instance.EndDay);
        DayManager.Instance.onDayBegan.AddListener(RefreshAll);
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshNudgeCount();
        if (dayText)    dayText.text    = $"Day {gameData.currentDay}";
        if (seasonText) seasonText.text = gameData.currentSeason.ToString();
        if (careNotificationText) careNotificationText.text = "";
    }

    public void RefreshNudgeCount()
    {
        if (nudgeCountText)
            nudgeCountText.text = $"Nudges: {gameData.nudgesRemainingToday}";
    }

    public void ShowCareNotification(string message)
    {
        if (careNotificationText)
            careNotificationText.text = message;
    }
}