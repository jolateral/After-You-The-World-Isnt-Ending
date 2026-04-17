using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    Dictionary<string, Transform> waypoints = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;
            if (waypoints.ContainsKey(child.name))
                Debug.LogWarning($"[Waypoints] Duplicate waypoint name: '{child.name}' — only the first will be used.");
            else
                waypoints[child.name] = child;
        }

        Debug.Log($"[Waypoints] Registered {waypoints.Count} waypoints.");
    }

    public Transform Get(string name)
    {
        if (waypoints.TryGetValue(name, out Transform t)) return t;
        Debug.LogError($"[Waypoints] Could not find waypoint: '{name}'. Check spelling and that it's a child of WaypointManager.");
        return null;
    }
}