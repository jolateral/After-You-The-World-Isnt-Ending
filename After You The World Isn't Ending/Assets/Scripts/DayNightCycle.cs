using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Tooltip("Time in minutes for a full day cycle")]
    public float minutesPerDay = 3f;

    public static float TimeOfDay { get; private set; }

    bool running = false;

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
        OnDayBegan();
    }

    void OnDayBegan()
    {
        TimeOfDay = 0f;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        running = true;
    }

    void OnDayEnded()
    {
        running = false;
    }

    void Update()
    {
        if (!running) return;

        float secondsPerDay = minutesPerDay * 60f;
        float delta = Time.deltaTime / secondsPerDay;
        TimeOfDay = Mathf.Clamp01(TimeOfDay + delta);

        float angle = TimeOfDay * 360f - 90f;
        transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }
}