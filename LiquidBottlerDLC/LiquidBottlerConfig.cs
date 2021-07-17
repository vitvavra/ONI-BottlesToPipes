using TUNING;
using UnityEngine;

namespace Alesseon.Building.Config
{
    public class LiquidBottlerConfig : IBuildingConfig
    {
        public const string ID = "Alesseon.LiquidBottler";


        public static readonly SimHashes[] enabledElements = new SimHashes[] {
            SimHashes.Water, SimHashes.DirtyWater, SimHashes.SaltWater,
            SimHashes.ViscoGel, SimHashes.Petroleum, SimHashes.Brine, SimHashes.CrudeOil,
            SimHashes.Ethanol, SimHashes.SuperCoolant, SimHashes.Naphtha, SimHashes.NuclearWaste
        };

        public override BuildingDef CreateBuildingDef()
        {

            int width = 3;
            int height = 2;
            string anim = "gas_bottler_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] raw_METALS = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;

            BuildingDef buildingDef = 
                BuildingTemplates.CreateBuildingDef(
                    ID,
                    width,
                    height,
                    anim,
                    hitpoints,
                    construction_time,
                    tier,
                    raw_METALS,
                    melting_point,
                    buildLocationRule,
                    BUILDINGS.DECOR.PENALTY.TIER1,
                    NOISE_POLLUTION.NOISY.TIER0,
                    0.2f
                )
            ;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.Floodable = false;
            buildingDef.Entombable = true;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.DefaultAnimState = "on";
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = 300f;
            storage.allowItemRemoval = false;
            storage.showInUI = true;
            go.AddOrGet<DropAllWorkable>();

            LiquidBottler liquidBottler = go.AddOrGet<LiquidBottler>();
            liquidBottler.storage = storage;
            liquidBottler.workTime = 9f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.storage = storage;
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.capacityKG = storage.capacityKg;
            conduitConsumer.keepZeroMassObject = false;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
        }
    }

}