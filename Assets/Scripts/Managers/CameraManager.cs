using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager
{

    private PostProcessVolume postProcessVolume;
    private Coroutiner coroutiner;

    private bool isVignetteAnimating = false;
    private Vignette vignette;
    private VignetteAnimationData currentVignetteAnimationData;
    
    public Camera MainCamera => Camera.main;

    public CameraManager(PostProcessVolume postProcessVolume, Coroutiner coroutiner)
    {
        this.postProcessVolume = postProcessVolume;
        this.coroutiner = coroutiner;
    }

    #region Vignette
    public void AnimateVignette(VignetteAnimationData animationData)
    {
        if (vignette == null)
        {
            Vignette tempVignette;
            if (postProcessVolume.profile.TryGetSettings(out tempVignette))
            {
                vignette = tempVignette;
            }
        }
        PlayVignetteAnimation(animationData);
    }

    private void PlayVignetteAnimation(VignetteAnimationData animationData)
    {
        if (animationData == null) return;

        if (!isVignetteAnimating)
        {
            currentVignetteAnimationData = animationData;
            coroutiner.RunCoroutine(AnimateVignette());
        }
        else if (currentVignetteAnimationData.Priority < animationData.Priority)
        {
            FinishVignetteAnimation();
            StartVignetteAnimation(animationData);
        }
    }

    private void StartVignetteAnimation(VignetteAnimationData animationData)
    {
        currentVignetteAnimationData = animationData;
        coroutiner.RunCoroutine(AnimateVignette());
    }


    private IEnumerator AnimateVignette()
    {
        isVignetteAnimating = true;
        vignette.enabled.overrideState = true;
        float timer = 0;

        while (timer < currentVignetteAnimationData.Duration || currentVignetteAnimationData.Indefinite)
        {
            timer += Time.deltaTime;
            ApplyToVignette(timer / currentVignetteAnimationData.Duration);
            Debug.Log(timer / currentVignetteAnimationData.Duration);
            yield return null;
        }

        FinishVignetteAnimation();
    }

    private void ApplyToVignette(float curveValue)
    {
        vignette.intensity.value = currentVignetteAnimationData.IntensityCurve.Evaluate(curveValue);
        vignette.smoothness.value = currentVignetteAnimationData.SmoothnessCurve.Evaluate(curveValue);
    }

    private void FinishVignetteAnimation()
    {
        ApplyToVignette(1);
        if (currentVignetteAnimationData.HideOnFinish)
        {
            vignette.enabled.overrideState = false;
        }
        currentVignetteAnimationData = null;
        isVignetteAnimating = false;
    }

    public void FinishVignetteAnimation(VignetteAnimationData animationData)
    {
        if (currentVignetteAnimationData != animationData) return;

        FinishVignetteAnimation();
    }
    #endregion
}
