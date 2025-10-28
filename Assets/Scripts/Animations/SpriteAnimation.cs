using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private List<AnimationData> animations;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float frameRate = 10f;

    private Dictionary<string, Sprite[]> animationDictionary;
    private Sprite[] currentFrames;
    private int currentFrame;
    private float timer;
    private Action animationCallback;

    private void Awake()
    {
        animationDictionary = new Dictionary<string, Sprite[]>();
        foreach (var animation in animations)
        {
            animationDictionary[animation.Key] = animation.Frames;
        }

        if (animations.Count > 0)
        {
            PlayDefault();
        }
    }

    public void PlayDefault()
    {

        PlayAnimation(animations[0].Key, PlayDefault);
    }

    private void Update()
    {
        if (currentFrames == null || currentFrames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
                currentFrame = (currentFrame + 1) % currentFrames.Length;
            if(currentFrame == 0)
            {
                animationCallback?.Invoke();
            }
            spriteRenderer.sprite = currentFrames[currentFrame];
        }
    }

    public void PlayAnimation(string key, Action onAnimationFinish)
    {
        if (animationDictionary.TryGetValue(key, out var frames))
        {
            currentFrames = frames;
            currentFrame = 0;
            timer = 0;
            animationCallback = onAnimationFinish;
        }
        else
        {
            Debug.LogWarning($"Animation with key '{key}' not found.");
        }
    }

    [Serializable]
    public struct AnimationData
    {
        [SerializeField]
        private string animationKey;
        [SerializeField]
        private Sprite[] sprites;

        public string Key => animationKey;
        public Sprite[] Frames => sprites;
    }
}
