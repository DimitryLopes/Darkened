using UnityEngine;

public class MazeButton : MissionItem
{
    [SerializeField]
    private Sprite unactiveSprite;
    [SerializeField]
    private Sprite activeSprite;
    public override Sprite Icon => spriteRenderer.sprite;

    public override void OnInteractionEnabled()
    {
        spriteRenderer.sprite = unactiveSprite;
    }

    public override void OnInteractionDisabled()
    {
        spriteRenderer.sprite = activeSprite;
    }
}
