using TMPro;
using UnityEngine;

namespace Scrapy.UI
{
    public class InteractUI : FadeCanvas
    {
        [SerializeField] private TMP_Text interactText;

        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.InteractableChanged += OnInteractableChanged;
        }

        private void OnInteractableChanged(Interactable newInteractable)
        {
            if (newInteractable == null)
            {
                SetOpen(false);
                return;
            }

            interactText.text = newInteractable.interactText;
            SetOpen(true);
        }
    }
}