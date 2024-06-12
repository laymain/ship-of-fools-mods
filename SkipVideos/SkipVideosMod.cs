using System.Reflection;
using MelonLoader;
using SkipVideos;

[assembly: AssemblyVersion(SkipVideosMod.Version)]
[assembly: MelonInfo(typeof(SkipVideosMod), SkipVideosMod.Name, SkipVideosMod.Version, SkipVideosMod.Author)]
[assembly: MelonGame("Fika Productions", "ShipOfFools")]

namespace SkipVideos;

public class SkipVideosMod : MelonMod
{
    internal const string Name = nameof(SkipVideosMod);
    internal const string Version = "0.0.2";
    internal const string Author = "Laymain";

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg($"Plugin {Name} by {Author} (version {Version}) loaded.");
    }

}
