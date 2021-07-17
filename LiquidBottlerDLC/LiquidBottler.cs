using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Alesseon.Building
{

    [AddComponentMenu("KMonoBehaviour/Workable/LiquidBottler")]
    class LiquidBottler : Workable
    {

        public Storage storage;
        private Controller.Instance smi;
        private bool permitAutoDrop = false;

        private static readonly EventSystem.IntraObjectHandler<LiquidBottler> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LiquidBottler>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi = new Controller.Instance(this);
            smi.StartSM();
            UpdateStoredItemState();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        protected override void OnCleanUp()
        {
            if (smi != null)
            {
                smi.StopSM(nameof(OnCleanUp));
            }
            base.OnCleanUp();
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(
                gameObject,
                permitAutoDrop ?
                    new KIconButtonMenu.ButtonInfo("action_bottler_autodrop", Strings.Get("ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.DENIED.NAME"), new System.Action(OnChangeAllowAutoDrop), tooltipText: Strings.Get("ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.DENIED.TOOLTIP"))
                :
                    new KIconButtonMenu.ButtonInfo("action_bottler_autodrop", Strings.Get("ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.ALLOWED.NAME"), new System.Action(OnChangeAllowAutoDrop), tooltipText: Strings.Get("ALESSEON.UI.USERMENUACTIONS.AUTO_PUMP_DROP.ALLOWED.TOOLTIP")), 0.4f)
                ;
        }

        private void OnChangeAllowAutoDrop()
        {
            permitAutoDrop = !permitAutoDrop;
            if (smi != null && smi.GetCurrentState() == smi.sm.ready)
            {
                smi.GoTo(smi.sm.filling);
            }
        }

        private void UpdateStoredItemState()
        {
            storage.allowItemRemoval = smi != null && smi.GetCurrentState() == smi.sm.ready;
            foreach (GameObject go in storage.items)
            {
                if (go != null)
                {
                    go.Trigger((int)GameHashes.OnStorageInteracted);
                }
            }
        }

        private class Controller : GameStateMachine<Controller, Controller.Instance, LiquidBottler>
        {
            public State empty;
            public State filling;
            public State ready;
            public State pickup;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = empty;

                empty.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, filling, smi => smi.master.storage.IsFull());
                filling.PlayAnim("working").OnAnimQueueComplete(ready);
                ready.
                    EventTransition(
                        GameHashes.OnStorageChange,
                        pickup,
                        smi => !smi.master.storage.IsFull()
                    ).Enter(
                        smi =>
                        {
                            smi.master.storage.allowItemRemoval = true;
                            if (smi.master.permitAutoDrop)
                            {
                                smi.master.storage.DropAll();
                                return;
                            }
                            foreach (GameObject go in smi.master.storage.items)
                            {
                                go.AddTag(GameTags.LiquidSource);
                                go.Trigger((int)GameHashes.OnStorageInteracted, smi.master.storage);
                            }
                        }
                    ).Exit(
                        smi =>
                        {
                            smi.master.storage.allowItemRemoval = false;
                            foreach (GameObject go in smi.master.storage.items)
                            {
                                go.Trigger((int)GameHashes.OnStorageInteracted, smi.master.storage);
                            }
                        }
                    );

                pickup.PlayAnim("pick_up").OnAnimQueueComplete(empty);
            }


            public new class Instance : GameInstance
            {
                public Instance(LiquidBottler master) : base(master) { }
            }

        }

    }
}
