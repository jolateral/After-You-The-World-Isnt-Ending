using UnityEngine;

[CreateAssetMenu(fileName = "Seq_New", menuName = "GhostGarden/CareSequence")]
public class CareSequence : ScriptableObject
{
    [Tooltip("All of these signals must have fired today")]
    public string[] requiredSignals;

    [Tooltip("Format: NPCName:EventName — all must have occurred today")]
    public string[] requiredNPCEvents;

    [Tooltip("What happens when this triggers")]
    public string careEventResult;
}