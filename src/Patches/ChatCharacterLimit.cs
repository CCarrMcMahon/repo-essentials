using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;

namespace RepoEssentials.src.patches;


public static class ChatCharacterLimit {
    public static ConfigEntry<int> CharacterLimit { get; private set; }
    public static ConfigEntry<bool> EnableTextWrapping { get; private set; }

    private static void LoadConfig(ConfigFile config) {
        CharacterLimit = config.Bind("Chat", "CharacterLimit", 50, "The maximum number of characters allowed in a chat messages.");
        EnableTextWrapping = config.Bind("Chat", "EnableTextWrapping", false, "Enable text wrapping in chat.");
    }

    private static void ChatManagerAwakePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Awake_Patch)).Patch();
        Plugin.Logger.LogDebug($"  - ChatManager_Awake_Patch: True");
    }

    private static bool ChatManagerStateActivePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_StateActive_Patch)).Patch();
        bool wasApplied = ChatManager_StateActive_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - ChatManager_StateActive_Patch: {wasApplied}");
        return wasApplied;
    }

    private static bool ApplyPatches(Harmony harmony) {
        bool allApplied = true;
        ChatManagerAwakePatch(harmony);
        allApplied &= ChatManagerStateActivePatch(harmony);
        return allApplied;
    }

    public static void Initialize(ConfigFile config, Harmony harmony) {
        Plugin.Logger.LogDebug("Loading ChatCharacterLimit config...");
        LoadConfig(config);
        Plugin.Logger.LogDebug("  > Success: True");

        Plugin.Logger.LogDebug("Applying ChatCharacterLimit patches...");
        bool patchLoaded = ApplyPatches(harmony);
        Plugin.Logger.LogDebug($"  > Success: {patchLoaded}");
    }
}


[HarmonyPatch(typeof(ChatManager), "Awake")]
public class ChatManager_Awake_Patch {
    static void Prefix(ChatManager __instance) {
        int characterLimit = ChatCharacterLimit.CharacterLimit.Value;
        Traverse.Create(__instance).Field("characterLimit").SetValue(characterLimit);
        Plugin.Logger.LogDebug($"ChatManager::characterLimit = {characterLimit}");

        // Get the chatText component
        var chatText = Traverse.Create(__instance).Field("chatText").GetValue<TextMeshProUGUI>();
        if (chatText != null) {
            // Configure text wrapping
            chatText.enableWordWrapping = ChatCharacterLimit.EnableTextWrapping.Value;
            Plugin.Logger.LogDebug($"ChatManager::chatText.enableWordWrapping = {chatText.enableWordWrapping}");
        } else {
            Plugin.Logger.LogError("Failed to find chatText component");
        }
    }
}


[HarmonyPatch(typeof(ChatManager), "StateActive")]
public class ChatManager_StateActive_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        bool foundNewLineChar = false;
        int getLengthIndex = -1;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (!foundNewLineChar) {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString().Equals("\n")) {
                    foundNewLineChar = true;
                }
            } else if (foundNewLineChar && getLengthIndex == -1) {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Equals("Int32 get_Length()")) {
                    getLengthIndex = i;
                }
            } else if (i == getLengthIndex + 1) {
                if (codes[i].opcode == OpCodes.Ldc_I4_S) {
                    codes[i] = new(OpCodes.Ldc_I4, ChatCharacterLimit.CharacterLimit.Value) { labels = codes[i].labels };
                    PatchSuccessful = true;
                    break;
                }
            }
        }

        // Return the original instructions if the patch failed
        if (!PatchSuccessful) {
            Plugin.Logger.LogError("Failed to find the expected instructions to patch.");
            return instructions;
        }

        return codes.AsEnumerable();
    }
}
