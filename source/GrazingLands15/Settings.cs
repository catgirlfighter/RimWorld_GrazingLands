using System;
using UnityEngine;
using Verse;

namespace GrazingLands
{
    public class Settings : ModSettings
    {
        private static float multiplier = 0f;
        private static float consumeChance = 0f;
        private static float yieldDamage = 0f;

        public static float Multiplier { get { return 1 + multiplier * 4; } }
        public static int ConsumeChance { get { return (int)Math.Round(consumeChance * 100); } }
        public static float YieldDamage { get { return 1 + yieldDamage * 4; } }

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Label("multiplierLabel".Translate((100 + Math.Round(multiplier, 3) * 400).ToString()));
            multiplier = listing_Standard.Slider(multiplier, 0f, 1f);
            listing_Standard.Label("consumeChanceLabel".Translate(Math.Round(consumeChance * 100).ToString()));
            consumeChance = listing_Standard.Slider(consumeChance, 0f, 1f);
            listing_Standard.Label("yieldDamageLabel".Translate((100 + Math.Round(yieldDamage, 3) * 400).ToString()));
            yieldDamage = listing_Standard.Slider(yieldDamage, 0f, 1f);
            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref multiplier, "multiplier", 0f, false);
            Scribe_Values.Look(ref consumeChance, "consumeChance2", 0.05f, false);
            Scribe_Values.Look(ref yieldDamage, "yieldDamage", 0.25f, false);
        }
    }
}
