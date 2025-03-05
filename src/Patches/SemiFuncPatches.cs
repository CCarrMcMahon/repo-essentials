using HarmonyLib;
using System.Globalization;

namespace RepoEssentials.src.patches {
    [HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.DollarGetString))]
    public class SemiFunc_DollarGetString_Patch {
        private static bool Prefix(int value, ref string __result) {
            __result = string.Format(CultureInfo.CurrentCulture, "{0:#,0}", value);
            return false;
        }
    }
}
