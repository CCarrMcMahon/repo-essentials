using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace RepoEssentials;


[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
[BepInProcess(GameInfo.EXECUTABLE_NAME)]
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger;

    private void Awake() {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo("Loading plugin...");

        // Configure Harmony
        Harmony harmony = new(PluginInfo.GUID);

        // Apply patches
        harmony.PatchAll();
        Logger.LogInfo("Harmony patches are loaded!");

        Logger.LogInfo("Plugin loaded successfully!");
    }
}
