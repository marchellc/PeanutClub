using BasicOverlays.Overlays;

using HarmonyLib;

using LabExtended.Core;
using LabExtended.API.Hints;

namespace mcx.Overlays.Patches
{
    public static class BasicOverlaysPatches
    {
        public static OverlayOptions? ServerNameOverlay { get; internal set; }

        [HarmonyPatch(typeof(PeanutClubOverlay), nameof(PeanutClubOverlay.OnUpdate))]
        private static bool UpdatePrefix(PeanutClubOverlay __instance)
        {
            if (ServerNameOverlay is null)
                return true;

            var field = AccessTools.Field(typeof(PeanutClubOverlay), "LocalData");
            var value = field.GetValue(__instance) as IEnumerable<HintData>;

            if (value is null)
            {
                ApiLog.Warn("Overlays", $"PeanutClubOverlay.LocalData is null");
                return true;
            }

            var data = value.ElementAtOrDefault(1);
            
            if (data is null)
            {
                ApiLog.Warn("Overlays", $"PeanutClubOverlay.LocalData[1] is null");
                return true;
            }

            if (!ServerNameOverlay.IsEnabled)
                data.Content = string.Empty;

            data.VerticalOffset = ServerNameOverlay.VerticalOffset;

            ServerNameOverlay = null;
            return true;
        }
    }
}