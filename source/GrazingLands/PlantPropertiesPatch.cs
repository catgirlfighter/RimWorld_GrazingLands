using Harmony;
using RimWorld;
using Verse;
using UnityEngine;

namespace GrazingLands
{
    class PlantPropertiesPatch
    {
        /* -- does nothing for some reason
        [HarmonyPatch(typeof(RimWorld.PlantProperties), "Harvestable", MethodType.Getter)]
        static class PlantProperties_Harvestable_GrazingLandsPatch
        {
            static void Postfix(ref PlantProperties __instance, ref bool __result)
            {
                Log.Message($"{__instance}={__instance.harvestTag}");
                if (__result && __instance.harvestTag == "Unharvestable")
                    __result = false;
            }
        }
        */
        static float RoundUp(float val)
        {
            if (val - Mathf.Floor(val) > 0)
                val = Mathf.Floor(val) + 1f;
            else
                val = Mathf.Floor(val);
            return val;
        }

        [HarmonyPatch(typeof(RimWorld.PlantProperties), "Sowable", MethodType.Getter)]
        static class PlantProperties_Sowable_GrazingLandsPatch
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

        [HarmonyPatch(typeof(RimWorld.Plant), "HarvestableNow", MethodType.Getter)]
        static class Plant_HarvestableNow_GrazingLandsPatch
        {
            static bool Prefix(ref Plant __instance, ref bool __result)
            {
                if (__instance.def.plant.harvestTag == "Unharvestable")
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RimWorld.Plant), "HarvestableSoon", MethodType.Getter)]
        static class Plant_HarvestableSoon_GrazingLandsPatch
        {
            static bool Prefix(ref Plant __instance, ref bool __result)
            {
                if (__instance.def.plant.harvestTag == "Unharvestable")
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RimWorld.Plant), "IngestedCalculateAmounts")]
        static class Plant_IngestedCalculateAmounts_GrazingLandsPatch
        {
            static bool Prefix(ref Plant __instance, Pawn ingester, float nutritionWanted, out int numTaken, out float nutritionIngested)
            {
                if (__instance.def.plant.harvestYield <= 0 || __instance.def.plant.harvestedThingDef == null || !__instance.def.plant.harvestedThingDef.IsNutritionGivingIngestible)
                {
                    numTaken = 0;
                    nutritionIngested = 0f;
                    return true;
                }
            
                //numTaken = 0;
                float maxAmount = RoundUp(__instance.def.plant.harvestYield * Mathf.Lerp(0.5f, 1f, __instance.HitPoints / __instance.MaxHitPoints));
                float needAmount = RoundUp(nutritionWanted / __instance.def.plant.harvestedThingDef.ingestible.CachedNutrition);

                if (__instance.def.plant.HarvestDestroys)
                {
                    maxAmount = Mathf.Min(maxAmount, needAmount);
                    nutritionIngested = maxAmount * __instance.def.plant.harvestedThingDef.ingestible.CachedNutrition;
                    float potentialDamage = Mathf.Lerp(0f, 1f, maxAmount / __instance.def.plant.harvestYield);
                    potentialDamage = __instance.MaxHitPoints * potentialDamage * 2f;
                    if (__instance.HitPoints - potentialDamage <= 0)
                    {
                        numTaken = 1;
                    }
                    else
                    {
                        numTaken = 0;
                        __instance.TakeDamage(new DamageInfo(DamageDefOf.Bite, potentialDamage, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                    }
                }
                else
                {
                    numTaken = 0;
                    maxAmount = Mathf.Min(maxAmount, needAmount * 2f);
                    nutritionIngested = maxAmount / 2f * __instance.def.plant.harvestedThingDef.ingestible.CachedNutrition;
                    float potentialDamage = Mathf.Lerp(0f, 1f, maxAmount / __instance.def.plant.harvestYield);
                    __instance.Growth -= potentialDamage;
                    if (__instance.Growth < 0.08f)
                    {
                        __instance.Growth = 0.08f;
                    }
                    if (__instance.Spawned)
                    {
                        __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlag.Things);
                    }
                }


                return false;
            }
        }

        [HarmonyPatch(typeof(RimWorld.Plant), "TickLong")]
        static class Plant_TickLong_GrazingLandsPatch
        {
            static void Postfix(ref Plant __instance)
            {
                if (!__instance.Destroyed && __instance.HitPoints < __instance.MaxHitPoints && !__instance.Dying)
                    if (GenLocalDate.DayTick(__instance.Map) / 2000 == 0 || __instance.GrowthRate > 1f && GenLocalDate.DayTick(__instance.Map) / 2000 == 15)
                    __instance.HitPoints += 1;
            }
        }
    }
}
