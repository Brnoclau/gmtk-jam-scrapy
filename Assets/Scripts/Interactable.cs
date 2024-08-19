using System.Collections.Generic;
using UnityEngine;

namespace Scrapy
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        public string interactText;

        private readonly List<Collider2D> _colliders = new();

        private void Start()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<Player.Player>() == null) return;
            _colliders.Add(other);
            UpdateInteractable();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponentInParent<Player.Player>() == null) return;
            if (_colliders.Contains(other)) _colliders.Remove(other);
            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            GameManager.Instance.NearbyInteractable = _colliders.Count > 0 ? this : null;
        }

        public abstract void Interact();
    }
}