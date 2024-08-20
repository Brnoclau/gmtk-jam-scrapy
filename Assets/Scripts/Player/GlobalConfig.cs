using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapy.Player
{
    
    [CreateAssetMenu(menuName = "Create GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        public List<PlayerComponentConfig> AllComponents;
        public AudioClips audio;
        public List<ActionHotkey> actionHotkeys;
        
        public static GlobalConfig Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }
    }

    [Serializable]
    public struct AudioClips
    {
        public AudioClip itemPickupClip;
        public AudioClip questComplete;
        public AudioClip buttonClick;
        public AudioClip openMap;
        public AudioClip enterWorkshop;
        public AudioClip exitWorkshopOrRespawn;
        public AudioClip placedComponent;
        public AudioClip deletedComponent;
        public AudioClip failedToPlaceComponent;
    }
}