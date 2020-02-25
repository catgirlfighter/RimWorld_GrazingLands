using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace GrazingLands
{
    public class Settings : ModSettings
    {
        public static float multiplier = 0f;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Label("multiplierLabel".Translate((100 + Math.Round(multiplier, 3) * 400).ToString()));
            multiplier = listing_Standard.Slider(multiplier, 0f, 1f);
            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref multiplier, "multiplier", 0f, false);
        }
    }
}
