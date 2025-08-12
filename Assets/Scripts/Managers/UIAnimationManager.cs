using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{

    public static UIAnimationManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Dictionary to store animations by GameObject
    private Dictionary<GameObject, List<UIAnimation>> activeAnimations = new Dictionary<GameObject, List<UIAnimation>>();

    /// <summary>
    /// Adds an animation to be managed by the manager.
    /// </summary>
    /// <param name="animation">The animation to be added.</param>
    public void AddAnimation(UIAnimation animation)
    {
        if (animation.AnimationTarget == null)
        {
            Debug.LogError("UIAnimationManager: Attempted to add an animation with no target.");
            return;
        }

        // Add the animation to the dictionary, grouped by the GameObject target
        if (!activeAnimations.ContainsKey(animation.AnimationTarget))
        {
            activeAnimations[animation.AnimationTarget] = new List<UIAnimation>();
        }

        activeAnimations[animation.AnimationTarget].Add(animation);
    }

    public void RemoveAnimation(UIAnimation animation)
    {
        if (animation.AnimationTarget == null)
        {
            Debug.LogError("UIAnimationManager: Attempted to remove an animation with no target.");
            return;
        }

        if (!activeAnimations.ContainsKey(animation.AnimationTarget)) return;

        activeAnimations[animation.AnimationTarget].Remove(animation);
        
        if (activeAnimations[animation.AnimationTarget].Count == 0)
        {
            activeAnimations.Remove(animation.AnimationTarget);
        }

        LeanTween.cancel(animation.AnimationTarget);
    }

    /// <summary>
    /// Cancels a specific animation by LeanTween ID (tween.id) on the GameObject.
    /// </summary>
    /// <param name="id">LeanTween ID</param>
    public void Cancel(GameObject target, int id, int priority)
    {
        if (!activeAnimations.ContainsKey(target)) return;
        
        List<UIAnimation> animations = activeAnimations[target];
        for (int i = 0; i < animations.Count; i++) { 

            if (animations[i].Tween.id == id)
            {
                Cancel(animations[i], priority);
                return;
            }
        }

        Debug.LogWarning($"UIAnimationManager: No animation found with \n ID {id}\n [Name] " + target.name);
    }

    /// <summary>
    /// Cancels all animations on a specific GameObject.
    /// </summary>
    /// <param name="target">The GameObject whose animations to cancel.</param>
    public void Cancel(GameObject target, int priority)
    {
        if (target == null || !activeAnimations.ContainsKey(target)) return;

        List <UIAnimation> animations = activeAnimations[target];
        for (int i = 0; i < animations.Count; i++)
        {
            Cancel(animations[i], priority);
        }

        activeAnimations.Remove(target);
    }

    public void Cancel(UIAnimation animation, int priority)
    {
        if (animation.IsPlaying && priority >= animation.Priority)
        { 
            LeanTween.cancel(animation.Tween.id, true);
            //Animation removes itself onComplete
        }
    }

    public void CancelAll()
    {
        for (int i = 0; i < activeAnimations.Keys.Count; i++)
        {
            GameObject target = activeAnimations.Keys.ElementAt(i);
            List<UIAnimation> animations = activeAnimations[target];

            for (int j = 0; j < animations.Count; j++)
            {
                if (animations[j].Tween != null)
                {
                    LeanTween.cancel(animations[j].Tween.id, true);
                }
            }
        }

        activeAnimations.Clear();
    }
}
