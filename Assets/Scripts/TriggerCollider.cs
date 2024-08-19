using System;
using UnityEngine;

namespace Scrapy
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        public event Action TriggerEntered;
        public event Action TriggerExited;

        private void Start()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((layerMask.value & (1 << other.transform.gameObject.layer)) == 0) return;
            TriggerEntered?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((layerMask.value & (1 << other.transform.gameObject.layer)) == 0) return;
            TriggerExited?.Invoke();
        }
    }
}