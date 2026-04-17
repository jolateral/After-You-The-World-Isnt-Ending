using UnityEngine;
using UnityEngine.Events;

public class NudgeReceiver : MonoBehaviour
{
    [Header("Identity")]
    public string nudgeId;

    [Header("Effect")]
    public UnityEvent onNudge;
    public string emitsSignal;

    [Header("Cooldown")]
    public float cooldownSeconds = 5f;
    float lastNudgeTime = -999f;

    public bool TryNudge()
    {
        if (Time.time - lastNudgeTime < cooldownSeconds) return false;
        if (!DayManager.Instance.gameData.UseNudge()) return false;

        lastNudgeTime = Time.time;
        onNudge.Invoke();

        if (!string.IsNullOrEmpty(emitsSignal))
            SequenceResolver.Instance?.ReceiveSignal(emitsSignal);

        UIManager.Instance?.RefreshNudgeCount();
        return true;
    }
}