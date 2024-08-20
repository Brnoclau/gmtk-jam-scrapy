using System;
using JetBrains.Annotations;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class JumperBit : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        public bool RegisterCollisions;

        // public event Action<CollisionDetails> Collided;
        public event Action Collided;


        private void Start()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((layerMask.value & (1 << other.transform.gameObject.layer)) == 0) return;
            if (other.isTrigger) return;
            RegisterCollision(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ((layerMask.value & (1 << other.transform.gameObject.layer)) == 0) return;
            if (other.isTrigger) return;
            RegisterCollision(other);
        }

        private void RegisterCollision(Collider2D other)
        {
            if (!RegisterCollisions) return;
            Debug.Log("JumperBit: Registering collision with " + other.name);
            Collided?.Invoke();
        }
    }
}