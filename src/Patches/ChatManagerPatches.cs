using BepInEx.Logging;
using HarmonyLib;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoEssentials.src.patches {
    [HarmonyPatch(typeof(ChatManager))]
    public class ChatManager_Update_Patch {
        private static MethodBase TargetMethod() {
            return typeof(ChatManager).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
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

            if (occurancesFound == 0) {
                Plugin.Logger.LogError("Failed to find the target method");
            } else if (occurancesFound != 2) {
                Plugin.Logger.LogError($"Found {occurancesFound} occurances of the target method instead of 2.");
            }

            return codes.AsEnumerable();
        }
    }
}
