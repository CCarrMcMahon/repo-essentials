using BepInEx.Configuration;
using HarmonyLib;

namespace RepoEssentials.src.patches;


public static class ChatCharacterLimit {
    public static ConfigEntry<int> CharacterLimit { get; private set; }

    private static void LoadConfig(ConfigFile config) {
        CharacterLimit = config.Bind("Chat", "CharacterLimit", 50, "The maximum number of characters allowed in a chat messages.");
    }

    private static bool ChatManagerAwakePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Awake_Patch)).Patch();
        bool wasApplied = ChatManager_Awake_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - ChatManager_Awake_Patch: {wasApplied}");
        return wasApplied;
    }

    private static bool ApplyPatches(Harmony harmony) {
        bool allApplied = true;
        allApplied &= ChatManagerAwakePatch(harmony);
        return allApplied;
    }

    public static void Initialize(ConfigFile config, Harmony harmony) {
        Plugin.Logger.LogDebug("Loading ChatCharacterLimit config...");
        LoadConfig(config);
        Plugin.Logger.LogDebug("ChatCharacterLimit config loaded!");

        Plugin.Logger.LogDebug("Applying ChatCharacterLimit patches...");
        bool patchLoaded = ApplyPatches(harmony);
        Plugin.Logger.LogDebug($"  > Success: {patchLoaded}");
    }
}


[HarmonyPatch(typeof(ChatManager), "Awake")]
public class ChatManager_Awake_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    static void Prefix(ChatManager __instance) {
        int characterLimit = ChatCharacterLimit.CharacterLimit.Value;
        Traverse.Create(__instance).Field("characterLimit").SetValue(characterLimit);
        Plugin.Logger.LogDebug($"Set ChatManager::characterLimit to {characterLimit}.");
        PatchSuccessful = true;
    }
}
