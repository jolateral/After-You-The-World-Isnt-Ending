using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Data")]
    public GameData gameData;

    [Header("Day Pacing")]
    public float dayDuration = 60f;

    [Header("Events")]
    public UnityEvent onDayBegan;
    public UnityEvent onDayEnded;
    public UnityEvent onSeasonChanged;
    public UnityEvent onYearComplete;

    bool dayRunning = false;
    Season lastSeason;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        lastSeason = gameData.currentSeason;
        BeginDay();
    }

    public void BeginDay()
    {
        gameData.ResetForNewDay();
        dayRunning = true;
        onDayBegan.Invoke();
        StartCoroutine(RunDay());
    }

    IEnumerator RunDay()
    {
        yield return new WaitForSeconds(dayDuration);
        if (dayRunning) EndDay();
    }

    public void EndDay()
    {
        if (!dayRunning) return;
        dayRunning = false;
        StopAllCoroutines();

        bool gardenWasCaredFor = SequenceResolver.Instance != null && SequenceResolver.Instance.WasCareTriggeredToday();

        foreach (var plant in gameData.plants)
        {
            if (gardenWasCaredFor)
                plant.ApplyCare();
            else
                plant.ApplyDailyDecay(0.08f);
        }

        SequenceResolver.Instance?.ResetForNewDay();
        onDayEnded.Invoke();

        gameData.AdvanceDay();

        if (gameData.currentSeason != lastSeason)
        {
            lastSeason = gameData.currentSeason;
            onSeasonChanged.Invoke();
        }

        if (gameData.currentDay > 365)
        {
            onYearComplete.Invoke();
            return;
        }
    }

    public void SkipDay()
    {
        EndDay();
    }
}