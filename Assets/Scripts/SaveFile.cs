using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Serialization;

namespace Script
{
    [Serializable]
    public struct SaveFile
    {
        public PlayerSaveData player;
        public List<string> reachedCheckpoints;
        public List<UnlockedComponentData> unlockedComponents;

        public static SaveFile Default()
        {
            return new SaveFile()
            {
                player = new PlayerSaveData()
                {
                    attachedComponents = new List<AttachedComponentSave>()
                }
            };
        }
    }

    [Serializable]
    public struct PlayerSaveData
    {
        public List<AttachedComponentSave> attachedComponents;
    }

    [Serializable]
    public struct UnlockedComponentData
    {
        public string key;
        public int maxCount;
    }
}