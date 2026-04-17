using UnityEngine;

[CreateAssetMenu(fileName = "Routine_New", menuName = "GhostGarden/DailyRoutine")]
public class DailyRoutine : ScriptableObject
{
    public Season season;
    public RoutineStep[] steps;
}

[System.Serializable]
public class RoutineStep
{
    [Tooltip("0 = start of day, 1 = end of day")]
    public float timeOfDay;
    public string waypointName;
    public string animationTrigger;
    public string broadcastEvent;
}