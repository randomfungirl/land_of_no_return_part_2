using UnityEngine;

public class SpriteColorFilter : MonoBehaviour
{
    [SerializeField] private Color tintColor = new Color(0.8f, 1f, 0.8f, 1f); // Светло-зелёный оттенок

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyTint();
    }

    public void ApplyTint()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = tintColor;
        }
    }

    // Для динамического изменения
    public void SetTintColor(Color newColor)
    {
        tintColor = newColor;
        ApplyTint();
    }
}