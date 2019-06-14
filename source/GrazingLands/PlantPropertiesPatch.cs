using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace GrazingLands
{
    class PlantPropertiesPatch
    {
        [HarmonyPatch(typeof(RimWorld.PlantProperties), "Harvestable", new Type[] {})]
        static class PlantProperties_Harvestable_GrazingLandsPatch
        {
            static bool Prefix(ref PlantProperties __instance, ref bool __result)
            {
                if (__instance.harvestTag == "Unharvestable")
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
