using System;
using System.Collections.Generic;

namespace Scrapy.Player
{
    public class BodyPlayerComponent : PlayerComponent
    {
        [NonSerialized] public List<PlayerComponent> AttachedComponents = new();
    }
}