using System;
using UnityEngine;

namespace Scrapy
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class PickableItem : MonoBehaviour
    {
        [SerializeField] protected string checkpointName;
        
        private void Start()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
            if (SaveManager.Instance.CurrentSave.reachedCheckpoints.Contains(checkpointName))
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<Player.Player>() != null)
            {
                OnPickup();
            }
        }

        protected virtual void OnPickup()
        {
            SaveManager.Instance.CurrentSave.reachedCheckpoints.Add(checkpointName);
            SaveManager.Instance.SaveGame();
            AnimateDestroy();
        }

        protected virtual void AnimateDestroy()
        {
            Destroy(gameObject);
        }
    }
}