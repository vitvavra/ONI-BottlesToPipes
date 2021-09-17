using TUNING;
using UnityEngine;
using System.Collections.Generic;

namespace Alesseon.Building.Config
{
    class LiquidBottleEmptierConfig: IBuildingConfig
    {
        public const string ID = "Alesseon.LiquidBottleEmptier";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR2_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues tieR2_2 = BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR1;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                3,
                "gas_emptying_station_kanim",
                30,
                60f,
                tieR2_1,
                refinedMetals,
                1600f,
                BuildLocationRule.OnFloor,
                tieR2_2,
                noise);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = true;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.capacityKg = 300f;
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<LiquidBottleEmptier>();
            ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
            dispenser.conduitType = ConduitType.Liquid;
            dispenser.elementFilter = LiquidBottlerConfig.enabledElements;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
