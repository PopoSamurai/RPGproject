using UnityEngine;

// Kontur na oryginalnym SpriteRenderer; materiał jest tworzony tylko raz.
// Run AFTER TacticalUnitSprite (DefaultExecutionOrder 10000), so sprite UVs are in sync.
[DefaultExecutionOrder(11000)]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class TacticalHoverWhiteOutline : MonoBehaviour
{
    [Min(0.005f)] public float outlineThickness = 0.035f;
    [Range(1f, 8f)] public float outlinePixels = 3f;

    private SpriteRenderer source;
    private Material originalMaterial;
    private Material outlineMaterial;
    private Sprite previousSprite;

    private void Awake() { source = GetComponent<SpriteRenderer>(); }

    private void OnEnable()
    {
        if (source == null) source = GetComponent<SpriteRenderer>();
        if (source == null) return;
        if (outlineMaterial == null)
        {
            Shader shader = Shader.Find("Tactical/AnimatedWhiteSpriteOutline");
            if (shader == null)
            {
                Debug.LogError("Brak shadera Tactical/AnimatedWhiteSpriteOutline.", this);
                enabled = false;
                return;
            }
            outlineMaterial = new Material(shader) { name = "Animated white outline (runtime)" };
        }
        // Zachowaj oryginalny material, nie zapamietuj wlasnego jako oryginalu.
        if (source.sharedMaterial != outlineMaterial)
            originalMaterial = source.sharedMaterial;
        source.sharedMaterial = outlineMaterial;
        previousSprite = null;
        Sync();
    }

    private void LateUpdate() { Sync(); }

    private void Sync()
    {
        if (source == null || outlineMaterial == null) return;
        Sprite sprite = source.sprite;
        float worldPixels = sprite != null ? outlineThickness * sprite.pixelsPerUnit : 3f;
        outlineMaterial.SetFloat("_OutlinePixels", Mathf.Clamp(Mathf.Max(outlinePixels, worldPixels), 1f, 8f));
        if (sprite == previousSprite) return;
        previousSprite = sprite;
        if (sprite == null) return;
        Texture texture = sprite.texture;
        Rect r = sprite.textureRect;
        outlineMaterial.SetVector("_SpriteUV", new Vector4(r.xMin / texture.width, r.yMin / texture.height,
            r.xMax / texture.width, r.yMax / texture.height));
    }

    private void OnDisable()
    {
        if (source != null && outlineMaterial != null && source.sharedMaterial == outlineMaterial)
            source.sharedMaterial = originalMaterial;
        // NIE niszcz materialu tutaj: hover moze wlaczac/wylaczac komponent.
    }

    private void OnDestroy()
    {
        if (source != null && outlineMaterial != null && source.sharedMaterial == outlineMaterial)
            source.sharedMaterial = originalMaterial;
        if (outlineMaterial != null) Destroy(outlineMaterial);
        outlineMaterial = null;
    }
}
