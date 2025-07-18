using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingDamageText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeDuration = 0.5f;
    private TextMeshPro text;
    private Color originalColor;

    private Transform mainCameraTransform;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        originalColor = text.color;
        mainCameraTransform = Camera.main.transform;
    }

    public void SetText(string value)
    {
        text.text = value;
        text.color = originalColor;
        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 1f; 

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            transform.LookAt(mainCameraTransform);
            transform.Rotate(0f, 180f, 0f); 

            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - t);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}