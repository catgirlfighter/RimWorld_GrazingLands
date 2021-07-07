using HarmonyLib;
using System.Reflection;
using Verse;

namespace GrazingLands
{
    [StaticConstructorOnStartup]
    public class GrazingLands : Mod
    {
        public static Settings Settings;

        public GrazingLands(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("net.avilmask.rimworld.mod.GrazingLands");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GetSettings<Settings>();
        }

        public void Save()
        {
            LoadedModManager.GetMod<GrazingLands>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "GrazingLands";
        }

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
