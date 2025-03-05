using HarmonyLib;
using System.Globalization;
using System.Reflection;

namespace RepoEssentials.src.patches;

public static class CurrentCulturePatch {
    public static void SemiFuncDollarGetStringPatch(Harmony harmony) {
        harmony.CreateClassProcessor(typeof(SemiFunc_DollarGetString_Patch)).Patch();
        Plugin.Logger.LogInfo("SemiFunc.DollarGetString patch applied.");
    }

    public static void ApplyPatches(Harmony harmony) {
        SemiFuncDollarGetStringPatch(harmony);
    }
}

[HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.DollarGetString))]
public class SemiFunc_DollarGetString_Patch {
    private static bool Prefix(int value, ref string __result) {
        __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
        return false;
    }
}
