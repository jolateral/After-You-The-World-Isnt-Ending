using UnityEngine;
using UnityEngine.InputSystem; // Added this to use the New Input System

public class NudgeSystem : MonoBehaviour
{
    public static NudgeSystem Instance { get; private set; }

    public Camera mainCamera;
    public float maxRayDistance = 20f;
    public LayerMask nudgeLayer;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if the Left Mouse Button was pressed this frame
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Get the current mouse position from the new system
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, nudgeLayer))
            {
                NudgeReceiver receiver = hit.collider.GetComponent<NudgeReceiver>();
                if (receiver != null)
                {
                    receiver.TryNudge();
                }
            }
        }
    }
}