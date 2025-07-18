using UnityEngine;
using UnityEngine.InputSystem;

public class MouseReticle : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 currentPos;

    public static Vector3 WorldPosition { get; private set; }


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
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        WorldPosition = new Vector3(worldPos.x, worldPos.y, 0f);

        if (canvas != null)
        {
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mouseScreenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out anchoredPos
            );

            currentPos = Vector2.Lerp(currentPos, anchoredPos, 0.15f);
            rectTransform.anchoredPosition = currentPos;
        }
    }
}