using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Photon.Pun;

namespace RepoEssentials.src.patches;


public static class SinglePlayerChatPatch {
    public static void ChatManagerUpdatePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Update_Patch)).Patch();
        Plugin.Logger.LogInfo("ChatManager patches applied.");
    }

    public static void PlayerAvatarUpdatePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(PlayerAvatar_Update_Patch)).Patch();
        harmony.CreateClassProcessor(typeof(PlayerAvatar_UpdateMyPlayerVoiceChat_Patch)).Patch();
        harmony.CreateClassProcessor(typeof(PlayerAvatar_ChatMessageSend_Patch)).Patch();
        harmony.CreateClassProcessor(typeof(PlayerAvatar_ChatMessageSpeak_Patch)).Patch();
        harmony.CreateClassProcessor(typeof(TTSVoice_TTSSpeakNow_Patch)).Patch();
        Plugin.Logger.LogInfo("PlayerAvatar patches applied.");
    }

    public static void ApplyPatches(Harmony harmony) {
        ChatManagerUpdatePatch(harmony);
        PlayerAvatarUpdatePatch(harmony);
    }
}


[HarmonyPatch(typeof(ChatManager), "Update")]
public class ChatManager_Update_Patch {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 2;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Equals("Boolean IsMultiplayer()")) {
                // Create a new instruction that loads 1 onto the stack so the next `brtrue.s` is always taken
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1) {
                    labels = codes[i].labels
                };

                // Replace the old instruction with our new one
                codes[i] = newInstruction;
                occurancesFound++;
            }
        }

        // Return the original instructions if the patch failed
        if (occurancesFound != EXPECTED_OCCURANCES) {
            Plugin.Logger.LogError(
                $"SinglePlayerChat Patch Failed: Found {occurancesFound} occurances of " +
                $"SemiFunc::IsMultiplayer() instead of {EXPECTED_OCCURANCES}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}


[HarmonyPatch(typeof(PlayerAvatar), "Update")]
public class PlayerAvatar_Update_Patch {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 3;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            // Search for occurances of `call bool GameManager::Multiplayer()`
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Equals("Boolean Multiplayer()")) {
                // Create a new instruction that loads 1 onto the stack so the next `brfalse` isn't taken
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1) {
                    labels = codes[i].labels
                };

                // Replace the old instruction with our new one
                codes[i] = newInstruction;
                occurancesFound++;
            } else if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Equals("Boolean get_IsMine()")) {
                // Add a `pop` instruction to remove the result of `get_IsMine()` from the stack
                CodeInstruction popInstruction = new(OpCodes.Pop);
                codes.Insert(i + 1, popInstruction);

                // Insert a `ldc.i4.1` instruction to load 1 onto the stack so the next `brfalse` isn't taken
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1);
                codes.Insert(i + 2, newInstruction);

                // Increment the index to skip the next instruction
                i += 2;
                occurancesFound++;
            }
        }

        // Return the original instructions if the patch failed
        if (occurancesFound != EXPECTED_OCCURANCES) {
            Plugin.Logger.LogError(
                $"SinglePlayerChat Patch Failed: Found {occurancesFound} occurances of " +
                $"GameManager::Multiplayer() instead of {EXPECTED_OCCURANCES}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}


[HarmonyPatch(typeof(PlayerAvatar), nameof(PlayerAvatar.UpdateMyPlayerVoiceChat))]
public class PlayerAvatar_UpdateMyPlayerVoiceChat_Patch {
    private static bool Prefix() {
        Plugin.Logger.LogDebug("PlayerAvatar::UpdateMyPlayerVoiceChat called.");
        return true;
    }
}


[HarmonyPatch(typeof(PlayerAvatar), nameof(PlayerAvatar.ChatMessageSend))]
public class PlayerAvatar_ChatMessageSend_Patch {
    private static bool Prefix(string _message, bool _debugMessage) {
        Plugin.Logger.LogDebug($"PlayerAvatar::ChatMessageSend called with message: {_message}");
        Plugin.Logger.LogDebug($"PlayerAvatar::ChatMessageSend called with debugMessage: {_debugMessage}");
        return true;
    }
}


[HarmonyPatch(typeof(PlayerAvatar), "ChatMessageSpeak")]
public class PlayerAvatar_ChatMessageSpeak_Patch {
    private static bool Prefix(PlayerAvatar __instance, string _message, bool crouching) {
        Plugin.Logger.LogDebug($"PlayerAvatar::ChatMessageSpeak called with message: {_message}");
        Plugin.Logger.LogDebug($"PlayerAvatar::ChatMessageSpeak called with crouching: {crouching}");

        PhotonView photonView = __instance.photonView;
        Plugin.Logger.LogDebug($"PlayerAvatar::ChatMessageSpeak called while photonView.IsMine = {photonView.IsMine}");
        return true;
    }
}


[HarmonyPatch(typeof(TTSVoice), nameof(TTSVoice.TTSSpeakNow))]
public class TTSVoice_TTSSpeakNow_Patch {
    private static bool Prefix(string text, bool crouch) {
        Plugin.Logger.LogDebug($"TTSVoice::TTSSpeakNow called with message: {text}");
        Plugin.Logger.LogDebug($"TTSVoice::TTSSpeakNow called with crouch: {crouch}");
        return true;
    }
}
