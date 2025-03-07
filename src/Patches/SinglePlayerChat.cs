using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace RepoEssentials.src.patches;


public static class SinglePlayerChat {
    private static bool ChatManagerUpdatePatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(ChatManager_Update_Patch)).Patch();
        bool patchSuccessful = ChatManager_Update_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - ChatManager_Update_Patch: {patchSuccessful}");
        return patchSuccessful;
    }

    private static bool ApplyPatches(Harmony harmony) {
        bool allApplied = true;
        allApplied &= ChatManagerUpdatePatch(harmony);
        return allApplied;
    }

    public static void Initialize(Harmony harmony) {
        Plugin.Logger.LogDebug("Applying SinglePlayerChat patches...");
        bool patchLoaded = ApplyPatches(harmony);
        Plugin.Logger.LogDebug($"  > Success: {patchLoaded}");
    }
}


[HarmonyPatch(typeof(ChatManager), "Update")]
public class ChatManager_Update_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int INSTR_COUNT = 2;
        int instrFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Equals("Boolean IsMultiplayer()")) {
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_1) { labels = codes[i].labels };

                // Replace the old instruction with our new one
                codes[i] = newInstruction;
                instrFound++;
            }
        }

        // Set the success flag based on whether we found the expected number of occurrences
        PatchSuccessful = instrFound == INSTR_COUNT;

        // Return the original instructions if the patch failed
        if (!PatchSuccessful) {
            Plugin.Logger.LogError(
                $"Found {instrFound} occurances of SemiFunc::IsMultiplayer() instead of {INSTR_COUNT}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}
