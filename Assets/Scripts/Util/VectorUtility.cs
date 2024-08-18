using UnityEngine;

namespace Scrapy.Util
{
    public static class VectorUtility
    {
        public static Vector2 XY(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
        public static Vector3 XY0(this Vector2 vector3)
        {
            return new Vector3(vector3.x, vector3.y, 0);
        }
    }
}