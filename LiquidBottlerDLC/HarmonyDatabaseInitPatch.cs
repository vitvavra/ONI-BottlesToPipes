using System.Collections.Generic;
using HarmonyLib;

namespace Alesseon.HarmonyDatabasePatch.LiquidBottleHandling
{

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class LiquidBottlesBuildingsPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static void Prefix()
        {
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                Building.Config.LiquidBottlerConfig.ID,
                "Liquid Bottle filler",
                "Allow Duplicants to fetch bottled liquids for delivery to buildings.",
                "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport."
            );
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                Building.Config.LiquidBottleEmptierConfig.ID,
                "Liquid bottle emptier",
                "Allows emptying bottles directly to the pipe system.",
                "Automatically empties <link=\"ELEMENTSLIQUID\">Liquid</link> from bottles for pipe transport."
            ) ;
            Strings.Add($"ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.DENIED.NAME", "Enable auto drop");
            Strings.Add($"ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.DENIED.TOOLTIP", "vczvxcvzxcv drop fluid");
            Strings.Add($"ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.ALLOWED.NAME", "Disable auto drop");
            Strings.Add($"ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.ALLOWED.TOOLTIP", "Auto drop disabled");

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

                //List<string> list = new List<string>(Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
                //list.Add(Building.Config.LiquidBottlerConfig.ID);
                //list.Add(Building.Config.LiquidBottleEmptierConfig.ID);
                //Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"] = list.ToArray();

                System.Reflection.FieldInfo info = typeof(Database.Techs).GetField("TECH_GROUPING");
                Dictionary<string, string[]> dict = (Dictionary<string, string[]>)info.GetValue(null);
                dict[TechID].Append(Building.Config.LiquidBottlerConfig.ID);
                dict[TechID].Append(Building.Config.LiquidBottleEmptierConfig.ID);
                typeof(Database.Techs).GetField("TECH_GROUPING").SetValue(null, dict);
            }
        }
    }
}
