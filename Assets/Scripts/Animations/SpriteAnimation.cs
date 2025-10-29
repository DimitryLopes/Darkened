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
    [SerializeField]
    private bool playOnAwake = false;

    private Dictionary<string, AnimationData> animationDictionary;
    private AnimationData currentAnimation;
    private int currentFrame;
    private float timer;
    private Action animationCallback;
    private bool isPlaying;

    private void Awake()
    {
        animationDictionary = new Dictionary<string, AnimationData>();
        foreach (var animation in animations)
        {
            animationDictionary[animation.Key] = animation;
        }

        if (animations.Count > 0 && playOnAwake)
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
        if (!isPlaying) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
                currentFrame = (currentFrame + 1) % currentAnimation.Frames.Length;
            if(currentFrame == 0)
            {
                isPlaying = false;
                animationCallback?.Invoke();
            }
            spriteRenderer.sprite = currentAnimation.Frames[currentFrame];
        }
    }

    public void PlayAnimation(string key, Action onAnimationFinish, bool reset = false)
    {
        if (currentAnimation.Key == key && !reset && isPlaying) return;

        if (animationDictionary.TryGetValue(key, out var animation))
        {
            isPlaying = true;
            currentAnimation = animation;
            currentFrame = 0;
            timer = 0;
            animationCallback = onAnimationFinish;
        }
        else
        {
            Debug.LogWarning($"Animation with key '{key}' not found.");
        }
    }

    public void FinishCurrent(bool playCallback)
    {
        if (!isPlaying) return;
        isPlaying = false;

        spriteRenderer.sprite = currentAnimation.Frames[^1];
        
        if (!playCallback) return;

        animationCallback?.Invoke();
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
