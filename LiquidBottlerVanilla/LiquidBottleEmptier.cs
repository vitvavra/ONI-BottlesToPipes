using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alesseon.Building
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class LiquidBottleEmptier : StateMachineComponent<LiquidBottleEmptier.StatesInstance>, IGameObjectEffectDescriptor
    {
        public float emptyRate = 2f;
        [Serialize]
        public bool allowManualPumpingStationFetching;
        [SerializeField]
        public Color noFilterTint = TreeFilterable.NO_FILTER_TINT;
        [SerializeField]
        public Color filterTint = TreeFilterable.NO_FILTER_TINT;
        private static readonly EventSystem.IntraObjectHandler<LiquidBottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LiquidBottleEmptier>((component, data) => component.OnRefreshUserMenu(data));
        private static readonly EventSystem.IntraObjectHandler<LiquidBottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LiquidBottleEmptier>((component, data) => component.OnCopySettings(data));

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        public List<Descriptor> GetDescriptors(GameObject go) => null;

        private void OnChangeAllowManualPumpingStationFetching()
        {
            allowManualPumpingStationFetching = !allowManualPumpingStationFetching;
            smi.RefreshChore();
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(gameObject, allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new System.Action(OnChangeAllowManualPumpingStationFetching), tooltipText: (UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new System.Action(OnChangeAllowManualPumpingStationFetching), tooltipText: (UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP)), 0.4f);
        }

        private void OnCopySettings(object data)
        {
            allowManualPumpingStationFetching = ((GameObject)data).GetComponent<LiquidBottleEmptier>().allowManualPumpingStationFetching;
            smi.RefreshChore();
        }

        public class StatesInstance :
          GameStateMachine<LiquidBottleEmptier.States, LiquidBottleEmptier.StatesInstance, LiquidBottleEmptier, object>.GameInstance
        {
            private FetchChore chore;

            public MeterController meter { get; private set; }

            public StatesInstance(LiquidBottleEmptier smi)
              : base(smi)
            {
                master.GetComponent<TreeFilterable>().OnFilterChanged += new System.Action<Tag[]>(OnFilterChanged);
                meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
                {
        "meter_target",
        "meter_arrow",
        "meter_scale"
                });
                Subscribe(-1697596308, new Action<object>(OnStorageChange));
                Subscribe(644822890, new Action<object>(OnOnlyFetchMarkedItemsSettingChanged));
            }

            public void CreateChore()
            {
                KBatchedAnimController component1 = GetComponent<KBatchedAnimController>();
                Tag[] tags = GetComponent<TreeFilterable>().GetTags();
                if (tags == null || tags.Length == 0)
                {
                    component1.TintColour = (Color32)master.noFilterTint;
                }
                else
                {
                    component1.TintColour = (Color32)master.filterTint;
                    Tag[] forbidden_tags;
                    if (!master.allowManualPumpingStationFetching)
                        forbidden_tags = new Tag[1]
                        {
            GameTags.LiquidSource
                        };
                    else
                        forbidden_tags = new Tag[0];
                    Storage component2 = GetComponent<Storage>();
                    chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component2, component2.Capacity(), GetComponent<TreeFilterable>().GetTags(), forbidden_tags: forbidden_tags);
                }
            }

            public void CancelChore()
            {
                if (chore == null)
                    return;
                chore.Cancel("Storage Changed");
                chore = null;
            }

            public void RefreshChore() => GoTo(sm.unoperational);

            private void OnFilterChanged(Tag[] tags) => RefreshChore();

            private void OnStorageChange(object data)
            {
                Storage component = GetComponent<Storage>();
                meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
            }

            private void OnOnlyFetchMarkedItemsSettingChanged(object data) => RefreshChore();

            public void StartMeter()
            {
                PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
                if (firstPrimaryElement == null)
                    return;
                firstPrimaryElement.KeepZeroMassObject = false;
                meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
                meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
                GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
            }

            private PrimaryElement GetFirstPrimaryElement()
            {
                Storage component1 = GetComponent<Storage>();

                for (int idx = 0; idx < component1.Count; ++idx)
                {
                    GameObject gameObject = component1[idx];
                    if (!(gameObject == null))
                    {
                        PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
                        if (!(component2 == null))
                            component2.KeepZeroMassObject = false;
                            return component2;
                    }
                }
                return null;
            }

            public void Emit(float dt)
            {
                PrimaryElement primaryElement = GetFirstPrimaryElement();
                if(primaryElement == null)
                    return;
                if(primaryElement.Mass == 0)
                {
                    //Storage storage = GetComponent<Storage>();
                    primaryElement.DeleteObject();
                }
            }
        }

        public class States :
          GameStateMachine<LiquidBottleEmptier.States, LiquidBottleEmptier.StatesInstance, LiquidBottleEmptier>
        {
            private StatusItem statusItem;
            public State unoperational;
            public State waitingfordelivery;
            public State emptying;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = waitingfordelivery;
                statusItem = new StatusItem(nameof(LiquidBottleEmptier), "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
                statusItem.resolveStringCallback = ((str, data) =>
                {
                    LiquidBottleEmptier bottleEmptier = (LiquidBottleEmptier)data;
                    if (bottleEmptier == null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
                });
                statusItem.resolveTooltipCallback = ((str, data) =>
                {
                    LiquidBottleEmptier bottleEmptier = (LiquidBottleEmptier)data;
                    if (bottleEmptier == null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
                });
                root.ToggleStatusItem(statusItem, (smi => smi.master));
                unoperational.TagTransition(GameTags.Operational, waitingfordelivery).PlayAnim("off");
                waitingfordelivery.TagTransition(GameTags.Operational, unoperational, true).EventTransition(GameHashes.OnStorageChange, emptying, (smi => !smi.GetComponent<Storage>().IsEmpty())).Enter("CreateChore", (smi => smi.CreateChore())).Exit("CancelChore", (smi => smi.CancelChore())).PlayAnim("on");
                emptying.TagTransition(GameTags.Operational, unoperational, true).EventTransition(GameHashes.OnStorageChange, waitingfordelivery, (smi => smi.GetComponent<Storage>().IsEmpty())).Enter("StartMeter", (smi => smi.StartMeter())).Update("Emit", (smi, dt) => smi.Emit(dt)).PlayAnim("working_loop", KAnim.PlayMode.Loop);
            }
        }
    }
}
