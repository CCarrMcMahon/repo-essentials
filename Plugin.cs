using BepInEx;
using BepInEx.Logging;

namespace RepoEssentials;

public static class RepoEssentialsInfo
{
    public const string REPO_NAME = "repo-essentials";
    public const string PROJECT_NAME = "RepoEssentials";
    public const string PLUGIN_NAME = "Essentials";
    public const string PLUGIN_GUID = "org.ccarrmcmahon.plugins.repo.essentials";
    public const string PLUGIN_VERSION = "1.0.0";
}


[BepInPlugin(RepoEssentialsInfo.PLUGIN_GUID, RepoEssentialsInfo.PLUGIN_NAME, RepoEssentialsInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {RepoEssentialsInfo.PLUGIN_GUID} is loaded!");
    }
}
