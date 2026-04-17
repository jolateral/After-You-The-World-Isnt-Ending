using UnityEngine;
using System.Collections.Generic;

public class SequenceResolver : MonoBehaviour
{
    public static SequenceResolver Instance { get; private set; }

    public CareSequence[] springSequences;
    public CareSequence[] summerSequences;
    public CareSequence[] fallSequences;
    public CareSequence[] winterSequences;

    List<string> signalsToday = new();
    List<string> npcEventsToday = new();
    bool careTriggeredToday = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ReceiveSignal(string signal)
    {
        signalsToday.Add(signal);
        Debug.Log($"[Sequence] Signal received: {signal}");
        CheckSequences();
    }

    public void ReceiveNPCEvent(string npcName, string eventName)
    {
        string key = $"{npcName}:{eventName}";
        npcEventsToday.Add(key);
        Debug.Log($"[Sequence] NPC event: {key}");
        CheckSequences();
    }

    void CheckSequences()
    {
        if (careTriggeredToday) return;

        CareSequence[] active = GetActiveSequences();
        foreach (var seq in active)
        {
            if (IsSequenceSatisfied(seq))
            {
                Debug.Log($"[Sequence] Care triggered: {seq.careEventResult}");
                careTriggeredToday = true;
                UIManager.Instance?.ShowCareNotification(seq.careEventResult);
                return;
            }
        }
    }

    bool IsSequenceSatisfied(CareSequence seq)
    {
        foreach (string required in seq.requiredSignals)
            if (!signalsToday.Contains(required)) return false;

        foreach (string required in seq.requiredNPCEvents)
            if (!npcEventsToday.Contains(required)) return false;

        return true;
    }

    CareSequence[] GetActiveSequences()
    {
        return DayManager.Instance.gameData.currentSeason switch
        {
            Season.Spring => springSequences,
            Season.Summer => summerSequences,
            Season.Fall   => fallSequences,
            Season.Winter => winterSequences,
            _             => new CareSequence[0]
        };
    }

    public bool WasCareTriggeredToday() => careTriggeredToday;

    public void ResetForNewDay()
    {
        signalsToday.Clear();
        npcEventsToday.Clear();
        careTriggeredToday = false;
    }
}