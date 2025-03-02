using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowsingPlus.Patches
{
    [HarmonyPatch(typeof(LevelsPanelView), nameof(LevelsPanelView.Awake))]
    public class LevelsPanelAwakePatch
    {
        public static void Postfix(LevelsPanelView __instance) {
            if (__instance.GetComponentInParent<UIRig>())
            {
                Core.levelsPanelOverride.Populate(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(LevelsPanelView), nameof(LevelsPanelView.Activate))]
    public class LevelsPanelEnablePatch
    {
        public static void Postfix(LevelsPanelView __instance)
        {
            Core.levelsPanelOverride.TryInitialize(__instance);
        }
    }

    [HarmonyPatch(typeof(AvatarsPanelView), nameof(AvatarsPanelView.Awake))]
    public class AvatarsPanelAwakePatch
    {
        public static void Postfix(AvatarsPanelView __instance)
        {
            if (__instance.GetComponentInParent<UIRig>())
            {
                Core.avatarPanelOverride.Populate(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(AvatarsPanelView), nameof(AvatarsPanelView.Activate))]
    public class AvatarsPanelEnablePatch
    {
        public static void Postfix(AvatarsPanelView __instance)
        {
            Core.avatarPanelOverride.TryInitialize(__instance);
        }
    }
}
