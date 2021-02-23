using Harmony;

namespace Alesseon.HarmonyDatabasePatch.LiquidBottleHandling
{

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class LiquidBottlesBuildingsPatch
    {
        private static void Postfix()
        {
            //LiquidBottler strings
            Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLER.NAME",
                    "Liquid Bottler"
                });
            Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLER.DESC",
                    "Allow Duplicants to fetch bottled liquids for delivery to buildings." //"This bottler station has access to: {Liquids}"
				});
            Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLER.EFFECT",
                    "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport." //"Liquid Available: {Liquids}"
				});
            //LiquidBottleEmptier strings
            Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLEEMPTIER.NAME",
                    "Liquid bottle emptier"
                });
            Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLEEMPTIER.DESC",
                    "Allows emptying bottles directly to the pipe system." //"This bottler station has access to: {Liquids}"
				});
            Strings.Add(new string[]{
                    "STRINGS.BUILDINGS.PREFABS.ALESSEON.LIQUIDBOTTLEEMPTIER.EFFECT",
                    "Automatically empties <link=\"ELEMENTSLIQUID\">Liquid</link> from bottles for pipe transport."
            });
            ModUtil.AddBuildingToPlanScreen("Plumbing", Building.Config.LiquidBottlerConfig.ID);
            ModUtil.AddBuildingToPlanScreen("Plumbing", Building.Config.LiquidBottleEmptierConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class LiquidBottlerDbPatch
    {
        private static void Postfix()
        {
            Db.Get().Techs.TryGet("ImprovedLiquidPiping")?.unlockedItemIDs.Add(Building.Config.LiquidBottlerConfig.ID);
            Db.Get().Techs.TryGet("ImprovedLiquidPiping")?.unlockedItemIDs.Add(Building.Config.LiquidBottleEmptierConfig.ID);
        }
    }
}
