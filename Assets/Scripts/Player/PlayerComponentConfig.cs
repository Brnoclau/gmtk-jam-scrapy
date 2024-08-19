using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("Wheel settings"), Min(0), Tooltip("Maximum wheel speed in degrees per second")]
        public float wheelSpeed = 180;

        [Tooltip("Maximum torque wheel can apply to reach it's maximum speed")]
        public float wheelMaxTorque = 100;
        
        [Tooltip("Represents how much torque is applied depending on wheel speed")]
        public AnimationCurve torqueCurve;

        [Tooltip("How much torque is applied when speed is in the opposite direction or player uses breaks"),Min(0)]
        public float breakTorque;

        // public float breakDamping = 100;
    }
}