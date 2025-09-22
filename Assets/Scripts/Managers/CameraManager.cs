using Zenject;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager
{
    private Volume postProcessVolume;
    private LeanTweenAnimationData finishVignetteIntensityAnimationData;
    private LeanTweenAnimationData finishVignetteSmoothnessAnimationData;
    private Vignette vignette;
    private VignetteAnimationData currentVignetteAnimationData;
    private bool IsVignetteAnimating => currentVignetteAnimationData != null;


    public CameraManager(Volume postProcessVolume, SignalBus signalBus)
    {
        this.postProcessVolume = postProcessVolume;

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
                finishVignetteIntensityAnimationData = new LeanTweenAnimationData(postProcessVolume.gameObject, vignette.intensity.value, 0, 0.5f, ApplyIntentsityToVignette);
                finishVignetteSmoothnessAnimationData = new LeanTweenAnimationData(postProcessVolume.gameObject, vignette.smoothness.value, 0, 0.5f, ApplyIntentsityToVignette);
            }
        }
        PlayVignetteAnimation(animationData);
    }

    private void PlayVignetteAnimation(VignetteAnimationData animationData)
    {
        if (animationData == null) return;

        if (IsVignetteAnimating && currentVignetteAnimationData.Priority >= animationData.Priority) return;

        FinishVignetteAnimation();
        StartVignetteAnimation(animationData);
    }

    private void StartVignetteAnimation(VignetteAnimationData animationData)
    {
        currentVignetteAnimationData = animationData;

        LeanTweenAnimationData itensityData = new LeanTweenAnimationData(
            postProcessVolume.gameObject,
            vignette.intensity.value,
            animationData.Intensity,
            animationData.Duration,
            ApplyIntentsityToVignette,
            FinishVignetteAnimation);

        LeanTweenAnimationData smoothnessData = new LeanTweenAnimationData(
            postProcessVolume.gameObject,
            vignette.smoothness.value,
            animationData.Smoothness,
            animationData.Duration,
            ApplySmoothnessToVignette,
            FinishVignetteAnimation);

        TweenUtils.DoTween(itensityData);
        TweenUtils.DoTween(smoothnessData, false);
    }


    private void ApplyIntentsityToVignette(float value)
    {
        vignette.intensity.value = value;
    }

    private void ApplySmoothnessToVignette(float value)
    {
        vignette.smoothness.value = value;
    }

    private void FinishVignetteAnimation()
    {
        if (currentVignetteAnimationData == null) return;

        TweenUtils.CancelTween(postProcessVolume.gameObject, false);

        if (!currentVignetteAnimationData.HideOnFinish) return;

        TweenUtils.DoTween(finishVignetteIntensityAnimationData);
        TweenUtils.DoTween(finishVignetteSmoothnessAnimationData, false);

        currentVignetteAnimationData = null;
    }

    private void ForceFinishVignetteAnimation()
    {
        if (currentVignetteAnimationData == null) return;

        TweenUtils.DoTween(finishVignetteIntensityAnimationData);
        TweenUtils.DoTween(finishVignetteSmoothnessAnimationData, false);

        currentVignetteAnimationData = null;
    }

    public void ForceFinishAllAnimations()
    {
        ForceFinishVignetteAnimation();
    }

    public void FinishVignetteAnimation(VignetteAnimationData animationData)
    {
        if (currentVignetteAnimationData != animationData) return;

        ForceFinishVignetteAnimation();
    }
    #endregion
}
