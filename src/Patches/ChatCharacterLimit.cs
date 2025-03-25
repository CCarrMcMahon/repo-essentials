using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;

namespace RepoEssentials.src.patches;


public static class ChatCharacterLimit {
    public static ConfigEntry<int> CharacterLimit { get; private set; }
    public static ConfigEntry<float> ChatTextWidth { get; private set; }
    public static ConfigEntry<float> CharacterSpacing { get; private set; }
    public static ConfigEntry<float> LineSpacing { get; private set; }

    private static void LoadConfig(ConfigFile config) {
        CharacterLimit = config.Bind(
            "Chat",
            "CharacterLimit",
            250,
            new ConfigDescription(
                "The maximum number of characters allowed in a chat messages.",
                new AcceptableValueRange<int>(0, 1000)
            )
        );
        ChatTextWidth = config.Bind(
            "Chat",
            "ChatTextWidth",
            525.0f,
            new ConfigDescription(
                "The width of the chat area.",
                new AcceptableValueRange<float>(-100.0f, 545.0f)
            )
        );
        CharacterSpacing = config.Bind(
            "Chat",
            "CharacterSpacing",
            -0.5f,
            new ConfigDescription(
                "The spacing between characters in chat.",
                new AcceptableValueRange<float>(-1.0f, 10.0f)
            )
        );
        LineSpacing = config.Bind(
            "Chat",
            "LineSpacing",
            -45.0f,
            new ConfigDescription(
                "The spacing between lines in chat.",
                new AcceptableValueRange<float>(-50.0f, 50.0f)
            )
        );
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
    public static bool PatchSuccessful { get; private set; } = false;

    static void Prefix(ChatManager __instance) {
        int characterLimit = ChatCharacterLimit.CharacterLimit.Value;
        Traverse.Create(__instance).Field("characterLimit").SetValue(characterLimit);

        // Get the chatText component
        TextMeshProUGUI chatText = Traverse.Create(__instance).Field("chatText").GetValue<TextMeshProUGUI>();
        if (chatText == null) {
            Plugin.Logger.LogError("Failed to find chatText component");
            return;
        }

        RectTransform chatRectTransform = chatText.GetComponent<RectTransform>();
        if (chatRectTransform == null) {
            Plugin.Logger.LogError("Failed to get RectTransform from chatText");
            return;
        }

        // Configure text wrapping
        chatText.enableWordWrapping = true;
        chatRectTransform.sizeDelta = new(ChatCharacterLimit.ChatTextWidth.Value, chatRectTransform.sizeDelta.y);

        // Configure text spacing
        chatText.characterSpacing = ChatCharacterLimit.CharacterSpacing.Value;
        chatText.lineSpacing = ChatCharacterLimit.LineSpacing.Value;

        PatchSuccessful = true;
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
