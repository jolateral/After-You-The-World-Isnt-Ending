using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [Header("Identity")]
    public string npcName;

    [Header("Routines — one per season")]
    public DailyRoutine[] seasonRoutines;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float waypointTolerance = 0.2f;

    Animator animator;
    Dictionary<string, Transform> waypointMap = new();
    DailyRoutine activeRoutine;
    Vector3 startPosition;

    void Awake()
    {
        Debug.Log("NPC " + npcName + " has awakened in the code!");
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        foreach (Transform child in FindObjectsByType<Transform>(FindObjectsSortMode.None))
            waypointMap[child.name] = child;
    }

    void Start()
    {
        // FAIL-SAFE: If the DayManager is already running the day by the time 
        // the NPC wakes up, manually trigger the DayBegan logic.
        if (DayManager.Instance != null && DayManager.Instance.isActiveAndEnabled)
        {
            // We use a small delay or check a 'dayRunning' variable
            // Since activeRoutine is null initially, this starts the NPC
            if (activeRoutine == null) 
            {
                OnDayBegan();
            }
        }
    }

    void OnEnable()
    {
        // We still subscribe just in case
        if (DayManager.Instance != null)
        {
            DayManager.Instance.onDayBegan.AddListener(OnDayBegan);
            DayManager.Instance.onDayEnded.AddListener(OnDayEnded);
        }
    }

    void OnDisable()
    {
        DayManager.Instance?.onDayBegan.RemoveListener(OnDayBegan);
        DayManager.Instance?.onDayEnded.RemoveListener(OnDayEnded);
    }

    void OnDayBegan()
    {
        transform.position = startPosition;
        Season season = DayManager.Instance.gameData.currentSeason;
        activeRoutine = null;
        foreach (var r in seasonRoutines)
            if (r.season == season) { activeRoutine = r; break; }

        if (activeRoutine != null)
            StartCoroutine(PlayRoutine());
    }

    void OnDayEnded()
    {
        StopAllCoroutines();
        transform.position = startPosition;
    }

IEnumerator PlayRoutine()
{
    Debug.Log(npcName + " started routine. Steps to do: " + activeRoutine.steps.Length);

    foreach (var step in activeRoutine.steps)
    {
        Debug.Log(npcName + " waiting for time: " + step.timeOfDay + ". Current time: " + DayNightCycle.TimeOfDay);

        while (DayNightCycle.TimeOfDay < step.timeOfDay)
        {
            yield return null;
        }

        Debug.Log(npcName + " time reached! Moving to: " + step.waypointName);

        if (waypointMap.TryGetValue(step.waypointName, out Transform wp))
        {
            yield return StartCoroutine(MoveTo(wp.position));
        }
        else
        {
            Debug.LogError(npcName + " COULD NOT FIND WAYPOINT: " + step.waypointName);
        }
    }
}

    IEnumerator MoveTo(Vector3 target)
    {
        animator?.SetBool("Walking", true);
        while (Vector3.Distance(transform.position, target) > waypointTolerance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
            yield return null;
        }
        animator?.SetBool("Walking", false);
    }

float GetRemainingDayTime()
    {
        // Calculate remaining time based on the 0-1 clock
        // If your dayDuration is in seconds, this tells you how many seconds are left.
        float totalDuration = DayManager.Instance.dayDuration;
        return totalDuration * (1f - DayNightCycle.TimeOfDay);
    }
}