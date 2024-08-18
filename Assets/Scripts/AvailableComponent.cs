using System;
using Scrapy.Player;

namespace Scrapy
{
    [Serializable]
    public class AvailableComponent
    {
        public PlayerComponentConfig componentConfig;
        public int count;
        public int maxCount;
    }
}