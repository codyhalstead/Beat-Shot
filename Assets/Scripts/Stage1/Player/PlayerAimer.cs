using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimer : MonoBehaviour
{
    [SerializeField] private Transform reticle; 
    [SerializeField] private float orbitRadius = 1.5f;

    private Camera mainCam;

    private void Start()
    {
        // Reference main cam
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Debug messages for failure
        if (mainCam == null)
        {
            Debug.LogError("Main camera not found");
            return;
        }
        if (Mouse.current == null)
        {
            Debug.LogError("Mouse.current is null");
            return;
        }
        if (reticle == null)
        {
            Debug.LogError("Reticle not assigned");
            return;
        }
        // Get mouse position in world space
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        // Calculate direction from player to mouse
        Vector3 direction = (mouseWorldPos - transform.position).normalized;
        // Position the orbiting aimer towards mouse direction
        reticle.position = transform.position + direction * orbitRadius;
        // Rotate the reticle so visual direction points towards mouse
        reticle.up = direction;
    }
}
