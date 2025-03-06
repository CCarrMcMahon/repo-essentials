using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace RepoEssentials.src.patches;


public static class SinglePlayerChatPatch {
    public static void ChatManagerUpdatePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Update_Patch)).Patch();
        Plugin.Logger.LogInfo("ChatManager patches applied.");
    }

    public static void ApplyPatches(Harmony harmony) {
        ChatManagerUpdatePatch(harmony);
    }
}


[HarmonyPatch(typeof(ChatManager), "Update")]
public class ChatManager_Update_Patch {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 2;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            var opcode = codes[i].opcode;
            string operand = codes[i].operand.ToString();

            if (opcode == OpCodes.Call && operand.Equals("Boolean IsMultiplayer()")) {
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1) { labels = codes[i].labels };

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
