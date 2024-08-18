using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Player;
using UnityEngine;

namespace Scrapy
{
    public class PickableComponents : PickableCheckpoint
    {
        [SerializeField] private List<ComponentEntry> components;

        protected override void OnPickup()
        {
            var unlockedComponents = SaveManager.Instance.CurrentSave.unlockedComponents;
            foreach (var component in components)
            {
                var existingUnlockIndex = unlockedComponents.FindIndex(x => x.key == component.config.key);
                if (existingUnlockIndex >= 0)
                {
                    unlockedComponents[existingUnlockIndex] = new UnlockedComponentData()
                    {
                        key = component.config.key,
                        maxCount = unlockedComponents[existingUnlockIndex].maxCount + component.count
                    };
                }
                else
                {
                    unlockedComponents.Add(new UnlockedComponentData()
                    {
                        key = component.config.key,
                        maxCount = component.count
                    });
                }
            }

            base.OnPickup();
        }
    }

    [Serializable]
    struct ComponentEntry
    {
        public PlayerComponentConfig config;
        [Min(1)] public int count;
    }
}