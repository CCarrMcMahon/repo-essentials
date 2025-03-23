using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace RepoEssentials.src;


[BepInPlugin(configs.PluginInfo.GUID, configs.PluginInfo.NAME, configs.PluginInfo.VERSION)]
[BepInProcess(configs.GameInfo.EXECUTABLE_NAME)]
[BepInIncompatibility("nickklmao.nolimitchatbox")]
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger;

    private void Awake() {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo("Loading plugin...");

        // Configure Harmony
        Harmony harmony = new(configs.PluginInfo.GUID);

        // Apply patches
        Logger.LogDebug("Loading Harmony patches...");
        patches.CurrentCulture.Initialize(harmony);
        patches.SinglePlayerChat.Initialize(harmony);
        patches.ChatCharacterLimit.Initialize(Config, harmony);
        patches.HostSettings.Initialize(harmony);
        Logger.LogDebug("Harmony patches loaded!");

        Logger.LogInfo("Plugin loaded successfully!");
    }
}
