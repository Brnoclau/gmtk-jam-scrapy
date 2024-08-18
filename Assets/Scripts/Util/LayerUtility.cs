using UnityEngine;

namespace Scrapy.Util
{
    public static class LayerUtility
    {
        public static void SetGameLayerRecursive(GameObject go, int layer)
        {
            var children = go.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }
    }
}