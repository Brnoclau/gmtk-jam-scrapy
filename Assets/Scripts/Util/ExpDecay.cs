using UnityEngine;

namespace Scrapy.Util
{
    public static class ExpDecay
    {
        public static float Decay(float a, float b, float decay, float dt)
        {
            return b + (a - b) * Mathf.Exp(-decay * dt);
        }
    }
}