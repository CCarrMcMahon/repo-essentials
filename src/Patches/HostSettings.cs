using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace RepoEssentials.src.patches;


public static class HostSettings {
    private static bool NetworkConnectTryJoiningRoomPatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(NetworkConnect_TryJoiningRoom_Patch)).Patch();
        bool patchSuccessful = NetworkConnect_TryJoiningRoom_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - NetworkConnect_TryJoiningRoom_Patch: {patchSuccessful}");
        return patchSuccessful;
    }

    private static bool SteamManagerHostLobbyPatch(Harmony harmony) {
        // Get the async state machine type for the HostLobby method
        var targetMethod = typeof(SteamManager).GetMethod("HostLobby", BindingFlags.Instance | BindingFlags.Public);
        var stateMachineAttr = targetMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
        var moveNextMethod = stateMachineAttr.StateMachineType.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic);

        // Create a transpiler for the MoveNext method
        HarmonyMethod transpiler = new(typeof(SteamManager_HostLobby_Patch), nameof(SteamManager_HostLobby_Patch.Transpiler));

        // Apply the patch
        harmony.Patch(moveNextMethod, transpiler: transpiler);
        bool patchSuccessful = SteamManager_HostLobby_Patch.PatchSuccessful;
        Plugin.Logger.LogDebug($"  - SteamManager_HostLobby_Patch: {patchSuccessful}");
        return patchSuccessful;
    }

    private static bool ApplyPatches(Harmony harmony) {
        bool allApplied = true;
        allApplied &= NetworkConnectTryJoiningRoomPatch(harmony);
        allApplied &= SteamManagerHostLobbyPatch(harmony);
        return allApplied;
    }

    public static void Initialize(Harmony harmony) {
        Plugin.Logger.LogDebug("Applying HostSettings patches...");
        bool patchLoaded = ApplyPatches(harmony);
        Plugin.Logger.LogDebug($"  > Success: {patchLoaded}");
    }
}


[HarmonyPatch(typeof(NetworkConnect), "TryJoiningRoom")]
public class NetworkConnect_TryJoiningRoom_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int INSTR_COUNT = 1;
        int instrFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (codes[i].opcode == OpCodes.Ldc_I4_6) {
                // Increase max players from 6 to 20
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_S, 20) { labels = codes[i].labels };

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
                $"Found {instrFound} occurrences of NetworkConnect::TryJoiningRoom instead of {INSTR_COUNT}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}


public class SteamManager_HostLobby_Patch {
    public static bool PatchSuccessful { get; private set; } = false;

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        const int INSTR_COUNT = 1;
        int instrFound = 0;

        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++) {
            if (codes[i].opcode == OpCodes.Ldc_I4_6) {
                // Increase max players from 6 to 20
                CodeInstruction newInstruction = new(OpCodes.Ldc_I4_S, 20) { labels = codes[i].labels };

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
                $"Found {instrFound} occurrences in SteamManager::HostLobby instead of {INSTR_COUNT}."
            );
            return instructions;
        }

        return codes.AsEnumerable();
    }
}
