using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Tooltip("Time in minutes for a full day cycle")]
    public float minutesPerRotation = 3f;

    // This is your "Game Clock" (0.0 to 1.0)
    public static float TimeOfDay { get; private set; }

    private float rotationDegrees;

    void Update()
    {
        float degreesPerSecond = 360f / (minutesPerRotation * 60f);
        float amountToRotate = degreesPerSecond * Time.deltaTime;

        // 1. Rotate the light
        transform.Rotate(Vector3.right * amountToRotate, Space.World);

        // 2. Track total rotation (0 to 360)
        rotationDegrees += amountToRotate;
        if (rotationDegrees >= 360f) rotationDegrees -= 360f;

        // 3. Convert to a 0-1 scale for your DailyRoutine
        // We use (rotation / 360) to get a percentage of the day passed
        TimeOfDay = rotationDegrees / 360f;
    }
}