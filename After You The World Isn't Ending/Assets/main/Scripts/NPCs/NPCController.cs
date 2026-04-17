using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("Identity")]
    public string npcName;

    [Header("Routines — one per season")]
    public DailyRoutine[] seasonRoutines;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float waypointTolerance = 0.15f;

    Animator animator;
    DailyRoutine activeRoutine;
    Vector3 startPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void OnEnable()
    {
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

    void Start()
    {
        if (activeRoutine == null)
            OnDayBegan();
    }

    void OnDayBegan()
    {
        StopAllCoroutines();
        transform.position = startPosition;

        Season season = DayManager.Instance.gameData.currentSeason;
        activeRoutine = null;

        foreach (var r in seasonRoutines)
        {
            if (r != null && r.season == season)
            {
                activeRoutine = r;
                break;
            }
        }

        if (activeRoutine == null)
        {
            Debug.LogWarning($"[NPC:{npcName}] No routine found for season {season}.");
            return;
        }

        if (activeRoutine.steps == null || activeRoutine.steps.Length == 0)
        {
            Debug.LogWarning($"[NPC:{npcName}] Routine '{activeRoutine.name}' has no steps.");
            return;
        }

        Debug.Log($"[NPC:{npcName}] Starting routine '{activeRoutine.name}' with {activeRoutine.steps.Length} steps.");
        StartCoroutine(PlayRoutine());
    }

    void OnDayEnded()
    {
        StopAllCoroutines();
        animator?.SetBool("Walking", false);
        transform.position = startPosition;
    }

    IEnumerator PlayRoutine()
    {
        foreach (var step in activeRoutine.steps)
        {
            Debug.Log($"[NPC:{npcName}] Waiting until time {step.timeOfDay:F2}. Current: {DayNightCycle.TimeOfDay:F2}");

            yield return new WaitUntil(() => DayNightCycle.TimeOfDay >= step.timeOfDay);

            Debug.Log($"[NPC:{npcName}] Time reached. Moving to '{step.waypointName}'.");

            if (!string.IsNullOrEmpty(step.waypointName))
            {
                Transform wp = WaypointManager.Instance?.Get(step.waypointName);
                if (wp != null)
                    yield return StartCoroutine(MoveTo(wp.position));
            }

            if (!string.IsNullOrEmpty(step.animationTrigger))
                animator?.SetTrigger(step.animationTrigger);

            if (!string.IsNullOrEmpty(step.broadcastEvent))
            {
                Debug.Log($"[NPC:{npcName}] Broadcasting '{step.broadcastEvent}'.");
                SequenceResolver.Instance?.ReceiveNPCEvent(npcName, step.broadcastEvent);
            }
        }

        Debug.Log($"[NPC:{npcName}] Routine complete.");
    }

    IEnumerator MoveTo(Vector3 target)
    {
        animator?.SetBool("Walking", true);

        while (Vector3.Distance(transform.position, target) > waypointTolerance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            Vector3 lookDir = new Vector3(target.x, transform.position.y, target.z);
            if (lookDir != transform.position)
                transform.LookAt(lookDir);

            yield return null;
        }

        animator?.SetBool("Walking", false);
        Debug.Log($"[NPC:{npcName}] Arrived at destination.");
    }
}