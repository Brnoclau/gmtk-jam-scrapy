using System.Collections.Generic;
using UnityEngine;

namespace Scrapy
{
    
    [CreateAssetMenu(menuName = "Create QuestsConfig", fileName = "QuestsConfig", order = 0)]
    public class QuestsConfig : ScriptableObject
    {
        public List<Quest> quests;
    }
}