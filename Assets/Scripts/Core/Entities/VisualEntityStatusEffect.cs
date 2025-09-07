using UnityEngine;

public class VisualEntityStatusEffect : Activateable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer baseRenderer;

    public void SetEffect(SpriteRenderer effectSpriteRenderer, Material effectMaterial)
    {
        transform.SetParent(effectSpriteRenderer.transform);
        transform.SetAsLastSibling();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.layer = effectSpriteRenderer.gameObject.layer;
        spriteRenderer.material = effectMaterial;
        spriteRenderer.sprite = effectSpriteRenderer.sprite;
        spriteRenderer.sortingLayerID = effectSpriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = effectSpriteRenderer.sortingOrder + 1;
    }

    private void Update()
    {        
        if (baseRenderer != null)
        {
            spriteRenderer.sprite = baseRenderer.sprite;
        }
    }
}
