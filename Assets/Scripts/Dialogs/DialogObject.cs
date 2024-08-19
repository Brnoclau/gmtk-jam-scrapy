using UnityEngine;

namespace Scrapy
{
    public class DialogObject : Interactable
    {
        [SerializeField] private DialogConfig dialogConfig;
        
        public override void Interact()
        {
            DialogManager.Instance.StartDialog(dialogConfig);
        }
    }
}