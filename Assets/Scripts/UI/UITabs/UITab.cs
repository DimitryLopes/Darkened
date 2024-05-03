public class UITab : Activateable
{
    private bool isAnimating;

    public override void Activate(bool forced = false)
    {
        if (active && !forced) return;

        active = true;
        DoActivateAnimation();
        gameObject.SetActive(true);

    }

    public void ForceDeactivate()
    {
        active = false;
        DoDeactivateAnimation();
    }

    public override void Deactivate()
    {
        if (IsActive)
        {
            active = false;
            DoDeactivateAnimation();
        }
    }

    private void DoActivateAnimation()
    {
        //OnActivateAnimationFinished
        OnActivate();
    }

    private void DoDeactivateAnimation()
    {
        //OnDeactivateAnimationFinished
        OnDeactivate();
        gameObject.SetActive(false);
    }
}
