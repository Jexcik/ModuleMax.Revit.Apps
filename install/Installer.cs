using System;
using System.IO;
using System.Linq;
using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using Assembly = System.Reflection.Assembly;

const string outputName = "ModuleMax.Revit.Apps";
const string projectName = "ModuleMax.Revit.Apps";

var iconsDirectory = ResolveIconsDirectory();

var project = new Project
{
    OutDir = "output",
    Name = projectName,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    GUID = new Guid("1358F4EF-295C-4BC2-92DF-142958908E46"),
    BannerImage = Path.Combine(iconsDirectory, "BannerImage.png"),
    BackgroundImage = Path.Combine(iconsDirectory, "BackgroundImage.png"),
    Version = Assembly.GetExecutingAssembly().GetName().Version.ClearRevision(),
    ControlPanelInfo =
    {
        Manufacturer = Environment.UserName,
        ProductIcon = Path.Combine(iconsDirectory, "ShellIcon.ico")
    }
};

var wixEntities = Generator.GenerateWixEntities(args);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

string ResolveIconsDirectory()
{
    var roots = new[]
    {
        AppContext.BaseDirectory,
        Environment.CurrentDirectory
    }
    .Select(Path.GetFullPath)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

    foreach (var root in roots)
    {
        var directory = new DirectoryInfo(root);
        while (directory is not null)
        {
            var installIcons = Path.Combine(directory.FullName, "install", "Resources", "Icons");
            if (Directory.Exists(installIcons))
            {
                return installIcons;
            }

            var localIcons = Path.Combine(directory.FullName, "Resources", "Icons");
            if (Directory.Exists(localIcons))
            {
                return localIcons;
            }

            directory = directory.Parent;
        }
    }

    throw new DirectoryNotFoundException(
        $"Cannot locate installer icons directory (install/Resources/Icons). BaseDirectory: '{AppContext.BaseDirectory}', CurrentDirectory: '{Environment.CurrentDirectory}'.");
}

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{project.Version}-SingleUser";
    project.Dirs =
    [
        new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{project.Version}-MultiUser";
    project.Dirs =
    [
        new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}