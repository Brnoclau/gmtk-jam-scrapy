using Scrapy.Player;
using UnityEngine;

namespace Scrapy
{
    [RequireComponent(typeof(Collider2D))]
    public class PickableCheckpoint : MonoBehaviour
    {
        [SerializeField] protected string checkpointName;
        [SerializeField] protected bool disableSound;
        
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
            QuestManager.Instance.CompleteCheckpoint(checkpointName);
            VfxManager.Instance.Play(GlobalConfig.Instance.vfx.itemPickup, transform.position);
            AnimateDestroy();
            if (!disableSound) SfxManager.Instance.Play(GlobalConfig.Instance.audio.itemPickupClip);
        }

        protected virtual void AnimateDestroy()
        {
            Destroy(gameObject);
        }
    }
}