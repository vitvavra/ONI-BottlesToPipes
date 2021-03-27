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

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi = new Controller.Instance(this);
            smi.StartSM();
            UpdateStoredItemState();

        }

        protected override void OnCleanUp()
        {
            if (smi != null)
            {
                smi.StopSM(nameof(OnCleanUp));
            }
            base.OnCleanUp();
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
                            foreach (GameObject go in smi.master.storage.items)
                            {
                                go.AddTag(GameTags.LiquidSource);
                                Console.WriteLine("HeheStatus: " + go.name);
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
