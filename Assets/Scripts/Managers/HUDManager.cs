using Zenject;

public class HUDManager
{
    private SignalBus signalBus;
    private HUD hud;

    [Inject]
    public HUDManager(HUD hud,SignalBus signalBus)
    {
        this.hud = hud;
        this.signalBus = signalBus;
    }

    public void ShowHud()
    {
        hud.Show();
    }

    public void HideHud()
    {
        hud.Hide();
    }
}
