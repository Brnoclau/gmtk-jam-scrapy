using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapy
{
    [Serializable]
    public class Quest
    {
        public string checkpointKey;
        public List<string> activeWhenCheckpointsComplete;
        
        [Tooltip("Is displayed when the quest is finished")] 
        public string name;
        [Tooltip("Is displayed on minimap UI")] 
        public string description;
        public bool playCompleteQuestAnimation = true;
        
        
        [NonSerialized] public bool complete;
    }
}