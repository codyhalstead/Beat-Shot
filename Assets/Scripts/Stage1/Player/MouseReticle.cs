using UnityEngine;
using UnityEngine.InputSystem;

public class MouseReticle : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 currentPos;

    private void Awake()
    {
        // Custom cursor position manager
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        // Hide default cursor
        Cursor.visible = false; 
    }

    private void LateUpdate()
    {
        // Get current mouse position
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // Convert mouse position to local UI space
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchoredPos
        );
        // Interpolate position (for smoothness)
        currentPos = Vector2.Lerp(currentPos, anchoredPos, 0.15f);
        // Put custom reticle at mouse position
        rectTransform.anchoredPosition = currentPos;
    }
}