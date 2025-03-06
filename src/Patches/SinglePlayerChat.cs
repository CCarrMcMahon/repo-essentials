using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace RepoEssentials.src.patches;


public static class SinglePlayerChatPatch {
    public static bool ChatManagerUpdatePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Update_Patch)).Patch();
        bool success = ChatManager_Update_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - ChatManager_Update_Patch: {success}");
        return success;
    }

    public static bool ApplyPatches(Harmony harmony) {
        Plugin.Logger.LogDebug("Applying SinglePlayerChatPatch...");
        bool success = ChatManagerUpdatePatch(harmony);
        Plugin.Logger.LogDebug($"  ==> {success}");
        return success;
    }
}


[HarmonyPatch(typeof(ChatManager), "Update")]
public class ChatManager_Update_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int EXPECTED_OCCURANCES = 2;
        int occurancesFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Equals("Boolean IsMultiplayer()")) {
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1) { labels = codes[i].labels };

                // Replace the old instruction with our new one
                codes[i] = newInstruction;
                occurancesFound++;
            }
        }

        // Set the success flag based on whether we found the expected number of occurrences
        PatchSuccessful = occurancesFound == EXPECTED_OCCURANCES;

        // Return the original instructions if the patch failed
        if (!PatchSuccessful) {
            Plugin.Logger.LogError(
                $"ChatManager_Update_Patch: Found {occurancesFound} occurances of " +
                $"SemiFunc::IsMultiplayer() instead of {EXPECTED_OCCURANCES}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}
