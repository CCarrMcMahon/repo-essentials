using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoEssentials.src.patches;

public static class SinglePlayerChatPatch {
    public static void ChatManagerUpdatePatch(Harmony harmony) {
        MethodInfo chatManagerUpdate = AccessTools.Method(typeof(ChatManager), "Update");
        MethodInfo chatManagerUpdatePatch = AccessTools.Method(typeof(ChatManager_Update_Patch), "Transpiler");
        harmony.Patch(chatManagerUpdate, transpiler: new HarmonyMethod(chatManagerUpdatePatch));
    }

    public static void PlayerAvatarUpdatePatch(Harmony harmony) {
        MethodInfo playerAvatarUpdate = AccessTools.Method(typeof(PlayerAvatar), "Update");
        MethodInfo playerAvatarUpdatePatch = AccessTools.Method(typeof(PlayerAvatar_Update_Patch), "Transpiler");
        harmony.Patch(playerAvatarUpdate, transpiler: new HarmonyMethod(playerAvatarUpdatePatch));
    }

    public static void ApplyPatches(Harmony harmony) {
        ChatManagerUpdatePatch(harmony);
        PlayerAvatarUpdatePatch(harmony);
    }
}

public class ChatManager_Update_Patch {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 2;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            // Search for occurances of `call bool SemiFunc::IsMultiplayer()`
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("IsMultiplayer")) {
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

public class PlayerAvatar_Update_Patch {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 1;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            // Search for occurances of `call bool GameManager::Multiplayer()`
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("Multiplayer")) {
                // Create a new instruction that loads 1 onto the stack so the next `brfalse` isn't taken
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
                $"GameManager::Multiplayer() instead of {EXPECTED_OCCURANCES}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}
