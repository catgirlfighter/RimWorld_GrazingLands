using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace GrazingLands
{
    class PlantPropertiesPatch
    {
        static float RoundUp(float val)
        {
            if (val - Mathf.Floor(val) > 0)
                val = Mathf.Floor(val) + 1f;
            else
                val = Mathf.Floor(val);
            return val;
        }

        [HarmonyPatch(typeof(RimWorld.Plant), "IngestedCalculateAmounts")]
        static class Plant_IngestedCalculateAmounts_GrazingLandsPatch
        {
            public static bool Prefix(ref Plant __instance, Pawn ingester, float nutritionWanted, out int numTaken, out float nutritionIngested)
            {
                if (__instance.def.plant.harvestedThingDef != null && !__instance.def.plant.harvestedThingDef.IsNutritionGivingIngestible)
                {
                    numTaken = 0;
                    nutritionIngested = 0f;
                    return true;
                }

                bool hasyield = __instance.def.plant.harvestYield > 0 && __instance.def.plant.harvestedThingDef != null;

                bool WildOrNonCrop = (!__instance.def.plant.Sowable || __instance.def.plant.harvestedThingDef == null) && __instance.def.IsIngestible && __instance.def.ingestible.foodType == FoodTypeFlags.Plant;
                bool HarvestDestroys = __instance.def.plant.HarvestDestroys && !WildOrNonCrop;

                float maxAmount;
                float harvestYield;
                float nutrition;

                if (hasyield)
                {
                    harvestYield = __instance.def.plant.harvestYield;
                    nutrition = __instance.def.plant.harvestedThingDef.ingestible.CachedNutrition;
                    maxAmount = RoundUp(harvestYield * Mathf.Lerp(0.5f, 1f, __instance.HitPoints / __instance.MaxHitPoints));
                }
                else
                {
                    harvestYield = 100;
                    nutrition = __instance.GetStatValue(StatDefOf.Nutrition, false) / harvestYield * Settings.Multiplier;
                    if (HarvestDestroys)
                        maxAmount = RoundUp(harvestYield * Mathf.Lerp(0.5f, 1f, __instance.HitPoints / __instance.MaxHitPoints));
                    else
                        maxAmount = RoundUp(harvestYield * __instance.Growth);
                }

                float needAmount = RoundUp(nutritionWanted / nutrition);

                if (HarvestDestroys)
                {
                    maxAmount = Mathf.Min(maxAmount, needAmount);
                    nutritionIngested = maxAmount * nutrition;

                    float potentialDamage = Mathf.Lerp(0f, 1f, maxAmount / harvestYield);
                    potentialDamage = __instance.MaxHitPoints * potentialDamage * Settings.YieldDamage;
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

                    if (hasyield)
                    {
                        maxAmount = Mathf.Min(maxAmount, needAmount * Settings.YieldDamage);
                        nutritionIngested = maxAmount / Settings.YieldDamage * nutrition;
                    } 
                    else
                    {
                        maxAmount = Mathf.Min(maxAmount, needAmount);
                        nutritionIngested = maxAmount * nutrition;
                    }

                    float potentialDamage = Mathf.Lerp(0f, 1f, maxAmount / harvestYield);
                    __instance.Growth -= potentialDamage;
                    if (__instance.Growth < 0.08f)
                    {
                        if (!hasyield && Settings.ConsumeChance > 0)
                            if (Settings.ConsumeChance == 100)
                                numTaken = 1;
                            else
                            {
                                _ = new Random();
                                int val = Random.Range(1, 100);
                                if (val <= Settings.ConsumeChance)
                                    numTaken = 1;
                            }

                            if (numTaken == 0)
                                __instance.Growth = 0.08f;
                    }

                    if (__instance.Spawned)
                    {
                        __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlagDefOf.Things);
                    }
                }

                return false;
            }
        }
    }
}
