﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace RepoEssentials.src;


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
        Logger.LogDebug("Loading Harmony patches...");
        patches.CurrentCulture.Initialize(harmony);
        patches.SinglePlayerChat.Initialize(harmony);
        patches.ChatCharacterLimit.Initialize(Config, harmony);
        Logger.LogDebug("Harmony patches loaded!");

        Logger.LogInfo("Plugin loaded successfully!");
    }
}
