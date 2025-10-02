using Nice3point.Revit.Toolkit.External;
using ModuleMax.Revit.Apps.Starter.Commands;

namespace ModuleMax.Revit.Apps.Starter;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        Host.Start();
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "ModuleMax.Revit.Apps.Starter");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/ModuleMax.Revit.Apps.Starter;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/ModuleMax.Revit.Apps.Starter;component/Resources/Icons/RibbonIcon32.png");
    }
}