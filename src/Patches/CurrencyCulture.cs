using HarmonyLib;
using System.Globalization;

namespace RepoEssentials.src.patches;


public static class CurrentCulturePatch {
    public static void SemiFuncDollarGetStringPatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(SemiFunc_DollarGetString_Patch)).Patch();
        Plugin.Logger.LogDebug("  - SemiFunc_DollarGetString_Patch: True");
    }

    public static void ApplyPatches(Harmony harmony) {
        Plugin.Logger.LogDebug("Applying CurrentCulturePatch...");
        SemiFuncDollarGetStringPatch(harmony);
        Plugin.Logger.LogDebug("  ==> True");
    }
}


[HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.DollarGetString))]
public class SemiFunc_DollarGetString_Patch {
    private static bool Prefix(int value, ref string __result) {
        __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
        return false;
    }
}
