using System.Collections.Generic;
using UnityEngine;

namespace Scrapy.Player
{
    
    [CreateAssetMenu(menuName = "Create GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        public List<PlayerComponentConfig> AllComponents;
        
        public static GlobalConfig Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }
    }
}