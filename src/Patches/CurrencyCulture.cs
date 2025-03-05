using HarmonyLib;
using System.Globalization;
using System.Reflection;

namespace RepoEssentials.src.patches;

public static class CurrentCulturePatch {
    public static void SemiFuncDollarGetStringPatch(Harmony harmony) {
        MethodInfo semiFuncDollarGetString = AccessTools.Method(typeof(SemiFunc), nameof(SemiFunc.DollarGetString));
        MethodInfo semiFuncDollarGetStringPatch = AccessTools.Method(typeof(SemiFunc_DollarGetString_Patch), "Prefix");
        harmony.Patch(semiFuncDollarGetString, prefix: new HarmonyMethod(semiFuncDollarGetStringPatch));
    }

    public static void ApplyPatches(Harmony harmony) {
        SemiFuncDollarGetStringPatch(harmony);
    }
}

public class SemiFunc_DollarGetString_Patch {
    private static bool Prefix(int value, ref string __result) {
        __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
        return false;
    }
}
