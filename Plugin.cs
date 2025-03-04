using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Globalization;

namespace RepoEssentials;

public static class RepoEssentialsInfo
{
    public const string REPO_NAME = "repo-essentials";
    public const string PROJECT_NAME = "RepoEssentials";
    public const string PLUGIN_GUID = "org.ccarrmcmahon.plugins.repo.essentials";
    public const string PLUGIN_NAME = "Essentials";
    public const string PLUGIN_VERSION = "0.1.0";
}

[HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.DollarGetString))]
public class DollarGetStringPatch
{
    private static bool Prefix(int value, ref string __result)
    {
        __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
        return false;
    }
}

[BepInPlugin(RepoEssentialsInfo.PLUGIN_GUID, RepoEssentialsInfo.PLUGIN_NAME, RepoEssentialsInfo.PLUGIN_VERSION)]
[BepInProcess("REPO.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {RepoEssentialsInfo.PLUGIN_GUID} is loaded!");

        // Configure Harmony
        Harmony harmony = new(RepoEssentialsInfo.PLUGIN_GUID);

        // Apply patches
        harmony.PatchAll();
        Logger.LogInfo("Harmony patches are loaded!");
    }
}
