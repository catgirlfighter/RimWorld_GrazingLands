using Harmony;
using System.Reflection;
using Verse;
using UnityEngine;
using RimWorld;

namespace GrazingLands
{
    [StaticConstructorOnStartup]
    public class GrazingLands : Mod
    {
        //#pragma warning disable 0649
        //        public static Settings Settings;
        //#pragma warning restore 0649

        public GrazingLands(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("net.avilmask.rimworld.mod.GrazingLands");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //GetSettings<Settings>();
        }

        //public void Save()
        //{
        //    LoadedModManager.GetMod<CommonSense>().GetSettings<Settings>().Write();
        //}

        //public override string SettingsCategory()
        //{
        //    return "CommonSense";
        //}

        //public override void DoSettingsWindowContents(Rect inRect)
        //{
        //    Settings.DoSettingsWindowContents(inRect);
        //}
    }
}
