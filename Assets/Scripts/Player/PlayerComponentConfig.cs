using UnityEngine;

namespace Scrapy.Player
{
    public enum PlayerComponentType
    {
        Body,
        Wheel,
        Thruster
    }
    [CreateAssetMenu(menuName = "Create PlayerComponentConfig", fileName = "PlayerComponentConfig", order = 0)]
    public class PlayerComponentConfig : ScriptableObject
    {
        public string key;
        public PlayerComponentType type;
        public GameObject prefab;
        public Sprite uiImage;
        public string uiName;
    }
}