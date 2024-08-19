using TMPro;
using UnityEngine;

namespace Scrapy.UI
{
    public class InteractUI : FadeCanvas
    {
        [SerializeField] private TMP_Text interactText;
        [SerializeField] private string enterWorkshopText = "Enter workshop";

        private bool _inWorkshop;
        private Interactable _interactable;

        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.InteractableChanged += OnInteractableChanged;
            GameManager.Instance.PlayerEnteredWorkshop += OnPlayerEnteredWorkshop;
            GameManager.Instance.PlayerExitedWorkshop += OnPlayerExitedWorkshop;
        }

        private void OnPlayerEnteredWorkshop()
        {
            _inWorkshop = true;
            interactText.text = enterWorkshopText;
            SetOpen(true);
        }

        private void OnPlayerExitedWorkshop()
        {
            _inWorkshop = false;
            if (_interactable != null)
            {
                interactText.text = _interactable.interactText;
            } else {
                SetOpen(false);
            }
        }

        private void OnInteractableChanged(Interactable newInteractable)
        {
            _interactable = newInteractable;
            if (_inWorkshop) return;
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