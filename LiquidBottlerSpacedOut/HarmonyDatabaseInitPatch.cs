using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using static Localization;

namespace Alesseon.LiquidBottler.HarmonyDatabasePatch
{

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class LiquidBottlesBuildingsPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static void Prefix()
        {
            ModUtil.AddBuildingToPlanScreen("Plumbing", Building.Config.LiquidBottlerConfig.ID);
            ModUtil.AddBuildingToPlanScreen("Plumbing", Building.Config.LiquidBottleEmptierConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class LiquidBottlerDbPatch
    {
        private const string TechID = "ImprovedLiquidPiping";
        private static void Postfix()
        {

            if (typeof(Database.Techs).GetField("TECH_GROUPING") == null)
            {
                Tech tech = Db.Get().Techs.TryGet(TechID);
                if (tech == null)
                    return;
                ICollection<string> list = (ICollection<string>)tech.GetType().GetField("unlockedItemIDs")?.GetValue(tech);
                if (list == null)
                    return;

                list.Add(Building.Config.LiquidBottlerConfig.ID);
                list.Add(Building.Config.LiquidBottleEmptierConfig.ID);
            }
            else
            {


                System.Reflection.FieldInfo info = typeof(Database.Techs).GetField("TECH_GROUPING");
                Dictionary<string, string[]> dict = (Dictionary<string, string[]>)info.GetValue(null);
                dict[TechID].Append(Building.Config.LiquidBottlerConfig.ID);
                dict[TechID].Append(Building.Config.LiquidBottleEmptierConfig.ID);
                typeof(Database.Techs).GetField("TECH_GROUPING").SetValue(null, dict);
            }
        }
    }

    [HarmonyPatch(typeof(Localization), "Initialize")]
    public class Localization_Initialize_Patch
    {
        public static void Postfix() => Translate(typeof(Alesseon.LiquidBottler.STRINGS));

        public static void Translate(System.Type root)
        {
            // Basic intended way to register strings, keeps namespace
            RegisterForTranslation(root);

            // Load user created translation files
            LoadStrings();

            // Register strings without namespace
            // because we already loaded user transltions, custom languages will overwrite these
            LocString.CreateLocStringKeys(root, null);

            // Creates template for users to edit
            GenerateStringsTemplate(root, System.IO.Path.Combine(KMod.Manager.GetDirectory(), "strings_templates"));
        }

        private static void LoadStrings()
        {
            string path = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "translations",
                GetLocale()?.Code + ".po"
            );
            System.Console.WriteLine(path);
            if (System.IO.File.Exists(path))
                OverloadStrings(LoadStringsFile(path, false));
        }
    }
}
