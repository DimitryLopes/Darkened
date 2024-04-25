public class DevScreenController : ScreenController
{
    public DevScreenController(DeveloperTools developerTools)
    {
        DeveloperTools = developerTools;
    }

    public DeveloperTools DeveloperTools { get; private set; }
}
