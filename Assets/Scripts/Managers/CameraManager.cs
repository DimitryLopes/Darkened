using System.Collections;
using UnityEngine;
using Zenject;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager
{

    private Volume postProcessVolume;
    private Coroutiner coroutiner;
    private SignalBus signalBus;

    private IEnumerator currentVignetteAnimation;
    private bool isVignetteAnimating = false;
    private Vignette vignette;
    private VignetteAnimationData currentVignetteAnimationData;
    
    public Camera MainCamera => Camera.main;

    public CameraManager(Volume postProcessVolume, Coroutiner coroutiner, SignalBus signalBus)
    {
        this.signalBus = signalBus;
        this.postProcessVolume = postProcessVolume;
        this.coroutiner = coroutiner;

        signalBus.Subscribe<OnGameCompletedSignal>(ForceFinishAllAnimations);
    }

    #region Vignette
    public void AnimateVignette(VignetteAnimationData animationData)
    {
        if (vignette == null)
        {
            Vignette tempVignette;
            if (postProcessVolume.profile.TryGet(out tempVignette))
            {
                vignette = tempVignette;
            }
        }
        PlayVignetteAnimation(animationData);
    }

    private void PlayVignetteAnimation(VignetteAnimationData animationData)
    {
        if (animationData == null) return;

        if (isVignetteAnimating)
        {
            FinishVignetteAnimation();
            return;
        }
        else if (isVignetteAnimating && currentVignetteAnimationData.Priority >= animationData.Priority) return;

        StartVignetteAnimation(animationData);

    }

    private void StartVignetteAnimation(VignetteAnimationData animationData)
    {
        currentVignetteAnimationData = animationData;
        currentVignetteAnimation = AnimateVignette();
        coroutiner.RunCoroutine(currentVignetteAnimation);
    }

    private IEnumerator AnimateVignette()
    {
        isVignetteAnimating = true;
        vignette.active = true;
        float timer = 0;

        while (timer < currentVignetteAnimationData.Duration)
        {
            timer += Time.deltaTime;
            ApplyToVignette(timer);
            yield return null;
        }

        FinishVignetteAnimation();
    }

    private void ApplyToVignette(float curveValue)
    {
        vignette.intensity.value = curveValue;
        vignette.smoothness.value = curveValue;
    }

    private void FinishVignetteAnimation()
    {
        if (currentVignetteAnimation == null) return;

        coroutiner.StopCoroutine(currentVignetteAnimation);
        currentVignetteAnimation = null;

        if (currentVignetteAnimationData.HideOnFinish)
        {
            vignette.active = false;
        }
        currentVignetteAnimationData = null;
        isVignetteAnimating = false;
    }

    public void ForceFinishAllAnimations()
    {
        FinishVignetteAnimation();
    }

    public void FinishVignetteAnimation(VignetteAnimationData animationData)
    {
        if (currentVignetteAnimationData != animationData) return;

        FinishVignetteAnimation();
    }
    #endregion
}
