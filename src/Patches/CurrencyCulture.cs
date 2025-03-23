using HarmonyLib;
using System.Globalization;

namespace RepoEssentials.src.patches;


public static class CurrentCulture {
    private static void SemiFuncDollarGetStringPatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(SemiFunc_DollarGetString_Patch)).Patch();
        Plugin.Logger.LogDebug("  - SemiFunc_DollarGetString_Patch: True");
    }

    private static void ApplyPatches(Harmony harmony) {
        SemiFuncDollarGetStringPatch(harmony);
    }

    public static void Initialize(Harmony harmony) {
        Plugin.Logger.LogDebug("Applying CurrentCulture patches...");
        ApplyPatches(harmony);
        Plugin.Logger.LogDebug($"  > Success: True");
    }
}


[HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.DollarGetString))]
public class SemiFunc_DollarGetString_Patch {
    private static bool Prefix(int value, ref string __result) {
        __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
        return false;
    }
}
